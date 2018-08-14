using Droplet.Data.Uow;
using Droplet.EventBus.Abstractions;
using Droplet.EventBus.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.EventBus.RabbitMQ
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {

        private readonly IRabbitMQConnectionManager _rabbitMQConnectionManager;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly int _retryCount;

        private readonly Dictionary<string, IModel> _consumerChannels;

        public RabbitMQEventBus(IRabbitMQConnectionManager rabbitMQConnectionManager, ILogger<RabbitMQEventBus> logger,
            IServiceProvider serviceProvider, IEventBusSubscriptionsManager subsManager, int retryCount = 5)
        {
            _rabbitMQConnectionManager = rabbitMQConnectionManager ?? throw new ArgumentNullException(nameof(rabbitMQConnectionManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _serviceProvider = serviceProvider;
            _retryCount = retryCount;
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;

            _consumerChannels = new Dictionary<string, IModel>();
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            var channel = _consumerChannels[eventName];
            channel.Close();
        }

        private RabbitMQEventAttribute GetRabbitMQEventAttribute(Type eventType)
        {
            var rabbitMQEventAttribute = eventType.GetCustomAttribute<RabbitMQEventAttribute>();
            if(rabbitMQEventAttribute == null)
            {
                throw new ArgumentException($"无效的IntegrationEvent，RabbitMQEvent必须使用RabbitMQEventAttribute", eventType.Name);
            }
            return rabbitMQEventAttribute;
        }

        private IRabbitMQPersistentConnection GetRabbitMQPersistentConnection(RabbitMQEventAttribute rabbitMQEventAttribute)
        {
            return _rabbitMQConnectionManager.Get(rabbitMQEventAttribute.TypeName);
        }

        public void Publish(IntegrationEvent @event)
        {
            var rabbitMQEventAttribute = GetRabbitMQEventAttribute(@event.GetType());
            var persistentConnection = GetRabbitMQPersistentConnection(rabbitMQEventAttribute);
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex.ToString());
                });

            using (var channel = persistentConnection.CreateModel())
            {
                var eventName = @event.GetType().Name;

                //channel.ExchangeDeclare(exchange: rabbitMQEventAttribute.Exchange,
                //                    type: "fanout");

                var body = @event.ToProtoBufBytes();

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    channel.BasicPublish(exchange: rabbitMQEventAttribute.Exchange,
                                     routingKey: rabbitMQEventAttribute.RoutingKey,
                                     mandatory:true,
                                     basicProperties: properties,
                                     body: body);
                });
            }
        }


        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            
            DoInternalSubscription(typeof(T));
            _subsManager.AddSubscription<T, TH>();
        }

        private void DoInternalSubscription(Type eventType)
        {
            var eventName = _subsManager.GetEventKey(eventType);

          
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                var channel = CreateConsumerChannel(eventType, eventName);
                _consumerChannels.Add(eventName,channel);
            }
        }

        private IModel CreateConsumerChannel(Type eventType,string eventName)
        {
            var rabbitMQEventAttribute = GetRabbitMQEventAttribute(eventType);
            var persistentConnection = GetRabbitMQPersistentConnection(rabbitMQEventAttribute);

            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var channel = persistentConnection.CreateModel();
            channel.QueueBind(queue: rabbitMQEventAttribute.Queue,
                                      exchange: rabbitMQEventAttribute.Exchange,
                                      routingKey: rabbitMQEventAttribute.RoutingKey);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var message = ea.Body;

                await ProcessEvent(eventName, message);

                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: rabbitMQEventAttribute.Queue,
                                 autoAck: false,
                                 consumer: consumer);


            return channel;
        }


        public void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            _subsManager.RemoveSubscription<T, TH>();
        }

       
        public void Dispose()
        {
            _rabbitMQConnectionManager.Dispose();
            _subsManager.Clear();
        }


        private async Task ProcessEvent(string eventName, byte[] message)
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var eventType = _subsManager.GetEventTypeByName(eventName);
                    var integrationEvent = message.DeserializeProtoBuf(eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    var handlers = scope.ServiceProvider.GetServices(concreteType);
                    foreach (var handler in handlers)
                    {
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                    await unitOfWork.CompleteAsync();
                }
            }
        }
    }
}
