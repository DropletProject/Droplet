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

        public ConsulServiceDiscovery(IConsulClient consul)
        {
            _consul = consul;
        }

        public async Task<IEnumerable<ServiceInformation>> GetServicesAsync(string name, string version = "")
        {
            if (!string.IsNullOrEmpty(version))
            {
                version = VersionHelper.GetVersion(version);
            }
            return await GetServicesFromConsulAsync(name, version);
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
       
    }
}
