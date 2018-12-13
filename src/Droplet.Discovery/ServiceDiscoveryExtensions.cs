using Droplet.Discovery.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Discovery
{
    public static class ServiceDiscoveryExtensions
    {
        public static async Task<ServiceInformation> GetServiceAsync(this IServiceDiscovery serviceDiscovery, IServiceSelector  serviceSelectionStrategy, string name, string version = "")
        {
            var services = await serviceDiscovery.GetServicesAsync(name, version);
            if(services== null || services.Count() == 0)
            {
                throw new ArgumentNullException($"{name}:{version} No service node was found");
            }

            return serviceSelectionStrategy.SelectAsync(services);
        }
        public static async Task<ServiceInformation> GetServiceAsync(this IServiceDiscovery serviceDiscovery,  string name, string version = "")
        {
            return await serviceDiscovery.GetServiceAsync(new PollingServiceSelector(), name, version);
        }
    }
}
