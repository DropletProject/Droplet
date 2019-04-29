using Droplet.Discovery;
using Droplet.Discovery.Selectors;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Droplet.ServiceProxy.Grpc.Internal;

namespace Droplet.ServiceProxy.Grpc
{
    public class ServiceProxy : IServiceProxy
    {
        private readonly IServicePool _servicePool;
        private readonly IEnumerable<CallInterceptor> _interceptors;


        public ServiceProxy(IServicePool servicePool,IEnumerable<CallInterceptor> interceptors)
        {
            _servicePool = servicePool;
            _interceptors = interceptors;
        }
        public async Task<TService> CreateAsync<TService>(string serviceName) where TService : ClientBase
        {
            var serviceInfo = await _servicePool.GetAsync(serviceName);
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


    public class ServiceProxy<TService> : IServiceProxy<TService> where TService : ClientBase
    {

        private readonly IServiceProxy _serviceProxy;
        private readonly IServiceNameSelector  _serviceNameSelector;

        public ServiceProxy(IServiceProxy serviceProxy, IServiceNameSelector serviceNameSelector)
        {
            _serviceProxy = serviceProxy;
            _serviceNameSelector = serviceNameSelector;
        }

        private TService _service;
        private object _lock = new object();

        public TService Service {
            get
            {
                if(_service == null)
                {
                    lock(_lock)
                    {
                        if(_service != null)
                        {
                            return _service;
                        }
                        _service = _serviceProxy.CreateAsync<TService>(_serviceNameSelector.Get(typeof(TService))).Result;
                    }
                }
                return _service;
            }
        }

        
    }

}
