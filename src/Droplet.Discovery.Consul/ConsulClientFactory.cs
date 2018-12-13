using Consul;
using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Discovery.Consul
{
    public class ConsulClientFactory
    {
        public static IConsulClient Create(ConsulDiscoveryConfiguration configuration)
        {
            return new ConsulClient(p =>
            {
                p.Address = configuration.Address;
            });
        }
    }
}
