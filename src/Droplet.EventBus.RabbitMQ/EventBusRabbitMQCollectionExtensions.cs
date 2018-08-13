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
    public static class EventBusRabbitMQCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddSingleton<IRabbitMQConnectionManager, RabbitMQConnectionManager>(sp => {

                var logger = sp.GetRequiredService<ILogger<RabbitMQConnectionManager>>();
                var options = sp.GetRequiredService<IOptions<EventBusRabbitMQOptions>>();
                return new RabbitMQConnectionManager(options.Value.Servers, logger, options.Value.RetryCount);
            });

            services.AddSingleton<IEventBus, RabbitMQEventBus>(sp =>
            {
                var rabbitMQConnectionManager = sp.GetRequiredService<IRabbitMQConnectionManager>();
                var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                var options = sp.GetRequiredService<IOptions<EventBusRabbitMQOptions>>();
                return new RabbitMQEventBus(rabbitMQConnectionManager, logger, sp, eventBusSubcriptionsManager, options.Value.RetryCount);
            });
            return services;
        }
    }

    public class EventBusRabbitMQOptions
    {
        public List<RabbitServerSetting> Servers { get; set; }

        public int RetryCount { get; set; }
    }
}
