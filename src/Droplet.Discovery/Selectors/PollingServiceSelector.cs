using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Droplet.Discovery.Selectors
{
   
    public class PollingServiceSelector : IServiceSelector
    {
        private Dictionary<string, int> _lastServiceIndex = new Dictionary<string, int>();

        private static object _lock = new object();

        public ServiceInformation SelectAsync(IEnumerable<ServiceInformation> services)
        {
            if(services == null || services.Count() == 0)
            {
                throw new ArgumentNullException(nameof(services));
            }
            string serviceName = services.First().Name;
            var orderedServices = services.OrderBy(s => s.Id).ToArray();

            lock(_lock)
            {
                int serviceIndex = 0;
                if (_lastServiceIndex.ContainsKey(serviceName))
                {
                    serviceIndex = _lastServiceIndex[serviceName];
                    if (serviceIndex >= orderedServices.Length - 1)
                    {
                        serviceIndex = 0;
                    }
                    else
                    {
                        serviceIndex++;
                    }
                    _lastServiceIndex[serviceName] = serviceIndex;
                }
                else
                {
                    _lastServiceIndex.Add(serviceName, 0);
                }
                return orderedServices[serviceIndex];
            }
            
        }
    }
}
