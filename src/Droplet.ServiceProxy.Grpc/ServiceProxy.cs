using Droplet.Discovery;
using Droplet.Discovery.Selectors;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Droplet.ServiceProxy.Grpc
{
    public class ServiceProxy : IServiceProxy
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IServiceSelector _serviceSelector;
        private readonly IEnumerable<CallInterceptor> _interceptors;


        public ServiceProxy(IServiceDiscovery serviceDiscovery, IServiceSelector serviceSelector, IEnumerable<CallInterceptor> interceptors)
        {
            _serviceDiscovery = serviceDiscovery;
            _serviceSelector = serviceSelector;
            _interceptors = interceptors;
        }
        public async Task<TService> CreateAsync<TService>(string serviceName) where TService : ClientBase
        {
            var serviceInfo = await _serviceDiscovery.GetServiceAsync(_serviceSelector, serviceName);
            if (_interceptors != null && _interceptors.Count() > 0)
            {
                return (TService)Activator.CreateInstance(typeof(TService), CreateChannel(serviceInfo).Intercept(_interceptors.ToArray()));
            }
            else
            {
                return (TService)Activator.CreateInstance(typeof(TService), CreateChannel(serviceInfo));
            }
        }

        private Channel CreateChannel(ServiceInformation serviceInfo)
        {
            return new Channel(serviceInfo.Host, serviceInfo.Port, ChannelCredentials.Insecure);
        }
    }
}
