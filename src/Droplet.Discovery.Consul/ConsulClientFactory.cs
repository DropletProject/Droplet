using Consul;
using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Discovery.Consul
{
    internal class ConsulClientFactory
    {
        public static IConsulClient Create(Uri address)
        {
            return new ConsulClient(p =>
            {
                p.Address = address;
            });
        }
    }
}
