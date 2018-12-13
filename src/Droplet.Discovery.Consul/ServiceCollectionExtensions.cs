using Droplet.Discovery;
using Droplet.Discovery.Consul;
using Droplet.Discovery.Selectors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsulDiscovery(this IServiceCollection services, Action<ConsulDiscoveryConfiguration> action)
        {
            var configuration = new ConsulDiscoveryConfiguration();
            action.Invoke(configuration);
            if(configuration.Address == null)
            {
                throw new ArgumentNullException(nameof(configuration.Address));
            }
            if(configuration.CacheRefreshInterval < 5)
            {
                throw new ArgumentOutOfRangeException(nameof(configuration.CacheRefreshInterval), "Must be greater than or equal to 5");
            }

            switch (configuration.ServiceSelectorType)
            {
                case ServiceSelectorType.Polling:
                    services.AddSingleton<IServiceSelector, PollingServiceSelector>();
                    break;
            }

            services.AddSingleton<IServiceRegistrar>(provider=> {
                return new ConsulServiceRegistrar(ConsulClientFactory.Create(configuration));
            });

            services.AddSingleton<IServiceDiscovery>(provider => {
                return new ConsulServiceDiscovery(ConsulClientFactory.Create(configuration), configuration);
            });

            return services;
        }
    }
}
