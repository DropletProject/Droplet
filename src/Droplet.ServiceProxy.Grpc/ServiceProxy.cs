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
            var callInvoker = new ServiceProxyCallInvoker(CreateChannel(serviceInfo));

            if (_interceptors != null && _interceptors.Count() > 0)
            {
                callInvoker.Intercept(_interceptors.ToArray());
            }
            return (TService)Activator.CreateInstance(typeof(TService), callInvoker);
        }

        private Channel CreateChannel(ServiceInformation serviceInfo)
        {
            return new Channel(serviceInfo.Host, serviceInfo.Port, ChannelCredentials.Insecure);
        }
    }
}
