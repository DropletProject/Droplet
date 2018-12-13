using Droplet.Discovery.Selectors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Discovery.Consul
{

    public class ConsulDiscoveryBuilder
    {
        /// <summary>
        /// consul address
        /// </summary>
        public Uri Address { get; private set; }

        /// <summary>
        /// Using Service Address Caching 
        /// </summary>
        public bool CacheAble { get; private set; } = false;
        /// <summary>
        /// Cache refresh interval (ms)
        /// </summary>
        public int CacheRefreshInterval { get; private set; } = 10;

        /// <summary>
        /// Service selection
        /// </summary>
        public Type ServiceSelectorType { get; private set; } = typeof(PollingServiceSelector);


        public ConsulDiscoveryBuilder(Uri address)
        {
            Address = address;
        }

        public ConsulDiscoveryBuilder UseCache(bool cacheAble = true)
        {
            CacheAble = cacheAble;
            return this;
        }

        public ConsulDiscoveryBuilder UseCache(int cacheRefreshInterval)
        {
            CacheRefreshInterval = cacheRefreshInterval;
            return UseCache(true);
        }

        public ConsulDiscoveryBuilder UseServiceSelector<TServiceSelector>() where TServiceSelector: IServiceSelector
        {
            ServiceSelectorType = typeof(TServiceSelector);
            return this;
        }
    }
}
