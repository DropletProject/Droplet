using Droplet.EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Droplet.EventBus.Abstractions;
using Droplet.EventBus;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RabbitMQCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQEventBus(this IServiceCollection services,Action<RabbitMQOptions> configure)
        {
            var rabbitMQOptions = new RabbitMQOptions();
            configure?.Invoke(rabbitMQOptions);
            services.AddSingleton(rabbitMQOptions);
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddSingleton<IRabbitMQConnectionManager, RabbitMQConnectionManager>();
            services.AddSingleton<IEventBus, RabbitMQEventBus>();
            return services;
        }
    }

   
}
