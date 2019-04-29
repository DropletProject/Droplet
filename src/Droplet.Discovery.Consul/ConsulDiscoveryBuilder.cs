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
        /// Cache refresh interval (s)
        /// </summary>
        public int CacheRefreshInterval { get; private set; } = 5;

        /// <summary>
        /// Service selection
        /// </summary>
        public Type ServiceSelectorType { get; private set; } = typeof(PollingServiceSelector);


        public ConsulDiscoveryBuilder(Uri address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }


        //public ConsulDiscoveryBuilder UsePool(int cacheRefreshInterval,Type serviceSelectorType)
        //{
        //    if(serviceSelectorType.IsAssignableFrom(typeof(IServiceSelector)))
        //    {
        //        throw new ArgumentException("serviceSelectorType must be implement IServiceSelector",nameof(serviceSelectorType));
        //    }
        //    if(cacheRefreshInterval < 5)
        //    {
        //        throw new ArgumentException("cacheRefreshInterval must be greater than 3", nameof(cacheRefreshInterval));
        //    }
        //    CacheRefreshInterval = cacheRefreshInterval;
        //    ServiceSelectorType = serviceSelectorType;
        //    PoolAble = true;
        //    return this;
        //}

        //public ConsulDiscoveryBuilder UsePool<TServiceSelector>(int cacheRefreshInterval) where TServiceSelector : IServiceSelector
        //{
        //    return UsePool(cacheRefreshInterval, typeof(TServiceSelector));
        //}

        //public ConsulDiscoveryBuilder UsePool(int cacheRefreshInterval = 5) 
        //{
        //    return UsePool(cacheRefreshInterval, typeof(PollingServiceSelector));
        //}


        /// <summary>
        /// UseCache
        /// </summary>
        /// <param name="cacheRefreshInterval">seconds</param>
        /// <returns></returns>
        public ConsulDiscoveryBuilder UseCache(int cacheRefreshInterval)
        {
            CacheRefreshInterval = cacheRefreshInterval;
            return this;
        }

        public ConsulDiscoveryBuilder UseServiceSelector<TServiceSelector>() where TServiceSelector : IServiceSelector
        {
            ServiceSelectorType = typeof(TServiceSelector);
            return this;
        }
    }
}
