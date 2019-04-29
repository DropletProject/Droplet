using Microsoft.Extensions.Logging;
using RawRabbit.Common;
using RawRabbit.Enrichers.MessageContext.Subscribe;
using RawRabbit.Operations.Subscribe.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RawRabbit;

namespace Droplet.RawRabbit.AutoSubscribe
{
    public class DefaultAutoSubscriber: IAutoSubscriber
    {
        private readonly IBusClient _bus;
        private readonly IAutoSubscriberMessageDispatcher _autoSubscriberMessageDispatcher;
        private readonly ILogger _logger;
        public DefaultAutoSubscriber(IBusClient bus, IAutoSubscriberMessageDispatcher autoSubscriberMessageDispatcher, ILogger<DefaultAutoSubscriber> logger)
        {
            _bus = bus;
            _autoSubscriberMessageDispatcher = autoSubscriberMessageDispatcher;
            _logger = logger;
        }


        /// <summary>
        /// 从程序集中找到自动订阅的类
        /// </summary>
        /// <param name="assemblies"></param>
        public virtual void Subscribe(params Assembly[] assemblies)
        {
            var classTypes = assemblies.SelectMany(
                a => a.GetTypes().Where(
                    t => t.GetTypeInfo().IsClass &&
                    !t.GetTypeInfo().IsAbstract
                    )
                );
            SubscribeAsyncConsumer(classTypes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TAsyncConsumer"></typeparam>
        public virtual void Subscribe<TMessage, TAsyncConsumer>()
            where TMessage : class
            where TAsyncConsumer : class, IConsumeAsync<TMessage>
        {
            InvokeAsyncMethod(new AutoSubscriberConsumerInfo(typeof(TAsyncConsumer), typeof(IConsumeAsync<TMessage>), typeof(TMessage)));
        }

        public void Subscribe(Type consumerType)
        {
            var interfaces = consumerType.GetInterfaces().Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IConsumeAsync<>));
            foreach (var inter in interfaces)
            {
                InvokeAsyncMethod(new AutoSubscriberConsumerInfo(consumerType, inter, inter.GetGenericArguments()[0]));
            }
        }

        /// <summary>
        /// 订阅异步消费者
        /// </summary>
        /// <param name="classTypes"></param>
        private void SubscribeAsyncConsumer(IEnumerable<Type> classTypes)
        {
            foreach (var autoSubscriberConsumerInfo in GetAutoSubscriberConsumerInfos(classTypes, typeof(IConsumeAsync<>)))
            {
                InvokeAsyncMethod(autoSubscriberConsumerInfo);
            }
        }

        /// <summary>
        /// 获取订阅消费者信息
        /// </summary>
        /// <param name="classTypes"></param>
        /// <param name="consumeType"></param>
        /// <returns></returns>
        private  List<AutoSubscriberConsumerInfo> GetAutoSubscriberConsumerInfos(IEnumerable<Type> classTypes, Type consumeType)
        {
            var interfaceTypes = new List<AutoSubscriberConsumerInfo>();
            foreach (var concreteType in classTypes.Where(t => t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract))
            {
                var interfaces = concreteType.GetInterfaces()
                    .Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == consumeType && !i.GetGenericArguments()[0].IsGenericParameter)
                    .Select(i=> new AutoSubscriberConsumerInfo(concreteType,i, i.GetGenericArguments()[0]));

                interfaceTypes.AddRange(interfaces);
            }
            return interfaceTypes;
        }

        /// <summary>
        /// 执行异步方法
        /// </summary>
        /// <param name="autoSubscriberConsumerInfo"></param>
        private  void InvokeAsyncMethod(AutoSubscriberConsumerInfo autoSubscriberConsumerInfo)
        {
            var dispatchMethod = _autoSubscriberMessageDispatcher
                .GetType()
                .GetMethod(nameof(IAutoSubscriberMessageDispatcher.DispatchAsync), BindingFlags.Instance | BindingFlags.Public)
                .MakeGenericMethod(autoSubscriberConsumerInfo.MessageType, autoSubscriberConsumerInfo.ConcreteType);

            var dispatchDelegate = dispatchMethod.CreateDelegate(
                typeof(Func<,,>).MakeGenericType(autoSubscriberConsumerInfo.MessageType, typeof(RetryMessageContext), typeof(Task<Acknowledgement>)),
                _autoSubscriberMessageDispatcher);

            var busSubscribeMethod = typeof(SubscribeMessageContextExtension).GetMethods()
                .Where(m => m.Name == nameof(SubscribeMessageContextExtension.SubscribeAsync))
                .Select(m => new { Method = m, Params = m.GetParameters() })
                .Single(m => m.Params.Length == 4
                    && m.Params[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,,>)
                    && m.Params[1].ParameterType.GetGenericArguments()[2] == typeof(Task<Acknowledgement>)
                   ).Method;
            var busSubscribeDelegate = busSubscribeMethod.MakeGenericMethod(autoSubscriberConsumerInfo.MessageType, typeof(RetryMessageContext));

            Action<ISubscribeContext> subscribeContextAction = ctx => ctx.UseMessageContext(c => new RetryMessageContext { RetryInfo = c.GetRetryInformation() });

            busSubscribeDelegate.Invoke(null, new object[] { _bus, dispatchDelegate, subscribeContextAction, null });
            _logger.LogInformation($"订阅了MQ消息:{autoSubscriberConsumerInfo.MessageType.FullName}");
        }

        
    }

    internal class AutoSubscriberConsumerInfo
    {
        public Type ConcreteType { get; }
        public Type InterfaceType { get; }
        public Type MessageType { get; }

        public AutoSubscriberConsumerInfo(Type concreteType, Type interfaceType, Type messageType)
        {
            ConcreteType = concreteType;
            InterfaceType = interfaceType;
            MessageType = messageType;
        }
    }
}
