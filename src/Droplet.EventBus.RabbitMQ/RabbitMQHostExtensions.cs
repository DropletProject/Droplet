using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Droplet.EventBus.Abstractions;

namespace Microsoft.Extensions.Hosting
{
    public static class RabbitMQHostExtensions
    {
        public static IHost UseRabbitMQEventBus(this IHost host)
        {
            var rabbitMQEventBus = host.Services.GetRequiredService<IEventBus>();
            var services = host.Services.GetService<IServiceCollection>();

            var eventHandlers = services.Where(s => s.ServiceType.IsGenericType && s.ServiceType.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));
            foreach (var eventHandler in eventHandlers)
            {
                var eventType = eventHandler.ServiceType.GetGenericArguments()[0];
                var subscribeMethod = rabbitMQEventBus.GetType().GetMethod("Subscribe").MakeGenericMethod(eventType, eventHandler.ImplementationType);
                subscribeMethod.Invoke(rabbitMQEventBus, null);
            }
            return host;
        }
    }
}
