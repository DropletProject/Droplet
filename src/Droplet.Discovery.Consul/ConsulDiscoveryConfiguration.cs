using Droplet.Discovery.Selectors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Discovery.Consul
{
    public class ConsulDiscoveryConfiguration
    {
        /// <summary>
        /// consul address
        /// </summary>
        public Uri Address { get; set; }

        /// <summary>
        /// Using Service Address Caching 
        /// </summary>
        public bool UseCache { get; set; } = false;
        /// <summary>
        /// Cache refresh interval (ms)
        /// </summary>
        public int CacheRefreshInterval { get; set; } = 60;

        /// <summary>
        /// Service selection
        /// </summary>
        public ServiceSelectorType ServiceSelectorType { get; set; } = ServiceSelectorType.Polling;
    }
}
