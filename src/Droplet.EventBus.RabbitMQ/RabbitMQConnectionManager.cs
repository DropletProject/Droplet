using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Droplet.EventBus.RabbitMQ
{
    public interface IRabbitMQConnectionManager:IDisposable
    {
        IRabbitMQPersistentConnection Get(string typeName);
    }

    public class RabbitMQConnectionManager: IRabbitMQConnectionManager
    {
        private readonly ConcurrentDictionary<string, IRabbitMQPersistentConnection> _rabbitMQPersistentConnections
            = new ConcurrentDictionary<string, IRabbitMQPersistentConnection>();

        private readonly List<RabbitServerSetting> _rabbitServerSettings;
        private readonly int _retryCount;
        private readonly ILogger _logger;

        public RabbitMQConnectionManager(RabbitMQOptions rabbitMQOptions , ILogger<RabbitMQConnectionManager> logger)
        {
            _rabbitServerSettings = rabbitMQOptions.Servers;
            _retryCount = rabbitMQOptions.RetryCount;
            _logger = logger;
        }

        public void Dispose()
        {
            _rabbitMQPersistentConnections.Values.ToList().ForEach((item) => item.Dispose());
        }

        public IRabbitMQPersistentConnection Get(string typeName)
        {
            return _rabbitMQPersistentConnections.GetOrAdd(typeName, (name) =>
            {
                var rabbitServerSetting = _rabbitServerSettings.FirstOrDefault(s => s.TypeName == typeName);

                var factory = new ConnectionFactory()
                {
                    HostName = rabbitServerSetting.Host,
                    Port = rabbitServerSetting.Port,
                    UserName = rabbitServerSetting.UserName,
                    Password = rabbitServerSetting.Password,
                    VirtualHost = rabbitServerSetting.VirtualHost,
                    RequestedHeartbeat = rabbitServerSetting.RequestedHeartbeat,
                };

                return new DefaultRabbitMQPersistentConnection(factory, _logger, _retryCount);
            });
        }
    }
}
