using Consul;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Discovery.Consul
{
    public class ConsulServiceDiscovery: IServiceDiscovery
    {
        private readonly IConsulClient _consul;
        private readonly ConcurrentDictionary<string, ServiceCache> _serviceCache = new ConcurrentDictionary<string, ServiceCache>();
       
        private readonly bool _useCache;
        private readonly int _cacheRefreshInterval;

        public ConsulServiceDiscovery(IConsulClient consul, bool useCache, int cacheRefreshInterval)
        {
            _consul = consul;
            _useCache = useCache;
            _cacheRefreshInterval = cacheRefreshInterval;
        }

        public async Task<IEnumerable<ServiceInformation>> GetServicesAsync(string name, string version = "")
        {
            if (!string.IsNullOrEmpty(version))
            {
                version = VersionHelper.GetVersion(version);
            }
            if(_useCache)
            {
                return await GetServicesByCacheAsync(name,version);
            }
            else
            {
                return await GetServicesFromConsulAsync(name,version);
            }
        }

        private async Task<IEnumerable<ServiceInformation>> GetServicesFromConsulAsync(string name, string version)
        {
            var queryResult = await _consul.Health.Service(name, tag: version, passingOnly: true);
            var services = queryResult.Response.Select(serviceEntry => new ServiceInformation
            {
                Name = serviceEntry.Service.Service,
                Id = serviceEntry.Service.ID,
                Host = serviceEntry.Service.Address,
                Port = serviceEntry.Service.Port,
                Version = VersionHelper.GetVersionFromTags(serviceEntry.Service.Tags),
                Tags = serviceEntry.Service.Tags
            });
            return services;
        }
        private async Task<IEnumerable<ServiceInformation>> GetServicesByCacheAsync(string name, string version)
        {
            if(!string.IsNullOrEmpty(version))
            {
                version = VersionHelper.GetVersion(version);
            }

            if(_serviceCache.TryGetValue(name,out ServiceCache serviceCache))
            {
                if(serviceCache.RefreshTime > DateTime.Now)
                {
                    return serviceCache.Services;
                }
            }
            var services = await GetServicesFromConsulAsync(name,version);
            var newCache = new ServiceCache(name, DateTime.Now.AddSeconds(_cacheRefreshInterval), services);
            _serviceCache.AddOrUpdate(name, newCache, (key, oldCache) => { return newCache; });
            return services;
        }

        class ServiceCache
        {
            public ServiceCache(string name,DateTime refreshTime, IEnumerable<ServiceInformation> services)
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
