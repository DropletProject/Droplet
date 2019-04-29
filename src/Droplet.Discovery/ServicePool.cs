using Droplet.Discovery.Selectors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Discovery
{

   
    public class ServicePool : IServicePool
    {

        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IServiceSelector _serviceSelector;
        private readonly ServicePoolOptions _options;

        private readonly ConcurrentDictionary<string, ServiceCache> _serviceCache = new ConcurrentDictionary<string, ServiceCache>();
        public ServicePool(IServiceDiscovery serviceDiscovery, IServiceSelector serviceSelector, ServicePoolOptions options)
        {
            _serviceDiscovery = serviceDiscovery;
            _serviceSelector = serviceSelector;
            _options = options;
        }

        public async Task<ServiceInformation> GetAsync(string name, string version = "")
        {
            var services = await GetServicesAsync(name,version);
            return _serviceSelector.SelectAsync(services);
        }

        private async Task<IEnumerable<ServiceInformation>> GetServicesAsync(string name, string version)
        {

            if (_serviceCache.TryGetValue(name, out ServiceCache serviceCache))
            {
                if (serviceCache.RefreshTime > DateTime.Now)
                {
                    return serviceCache.Services;
                }
            }
            var services = await _serviceDiscovery.GetServicesAsync(name, version);
            var newCache = new ServiceCache(name, DateTime.Now.AddSeconds(_options.CacheRefreshInterval), services);
            _serviceCache.AddOrUpdate(name, newCache, (key, oldCache) => { return newCache; });
            return services;
        }

        class ServiceCache
        {
            public ServiceCache(string name, DateTime refreshTime, IEnumerable<ServiceInformation> services)
            {
                Name = name;
                RefreshTime = refreshTime;
                Services = services;
            }
            public string Name { get; set; }

            public DateTime RefreshTime { get; set; }

            public IEnumerable<ServiceInformation> Services { get; set; }
        }
    }
}
