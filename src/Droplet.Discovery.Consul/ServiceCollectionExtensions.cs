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
        public static IServiceCollection AddConsulDiscovery(this IServiceCollection services,Uri address, Action<ConsulDiscoveryBuilder> action = null)
        {
            var builder = new ConsulDiscoveryBuilder(address);
            action?.Invoke(builder);

            if (builder.Address == null)
            {
                throw new ArgumentNullException(nameof(builder.Address));
            }
            if (builder.CacheRefreshInterval < 5)
            {
                throw new ArgumentOutOfRangeException(nameof(builder.CacheRefreshInterval), "Must be greater than or equal to 5");
            }

            services.AddSingleton<IServiceRegistrar>(provider => {
                return new ConsulServiceRegistrar(ConsulClientFactory.Create(builder.Address));
            });

            services.AddSingleton<IServiceDiscovery>(provider => {
                return new ConsulServiceDiscovery(ConsulClientFactory.Create(builder.Address));
            });

            services.AddSingleton(typeof(IServiceSelector), builder.ServiceSelectorType);

            services.AddSingleton<IServicePool>(provider => {
                var options = new ServicePoolOptions() { CacheRefreshInterval = builder.CacheRefreshInterval };
                return new ServicePool(provider.GetService<IServiceDiscovery>(), provider.GetService<IServiceSelector>(), options);
            });

            return services;
        }
    }
}
