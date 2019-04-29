using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Droplet.ServiceProxy.Grpc
{
 

    public class GrpcServiceProxyBuilder
    {
        /// <summary>
        /// grpc client Interceptor
        /// </summary>
        public List<Type> CallInterceptorTypes { get; private set; } = new List<Type>();

        public IDictionary<Type, string> ClientDic { get; private set; } = new Dictionary<Type, string>();

        public GrpcServiceProxyBuilder UseCallInterceptors(params Type[] callInterceptorTypes)
        {
            CallInterceptorTypes = callInterceptorTypes.ToList();
            return this;
        }

        public GrpcServiceProxyBuilder UseCallInterceptor<TCallInterceptor>() where TCallInterceptor: CallInterceptor
        {
            CallInterceptorTypes.Add(typeof(TCallInterceptor));
            return this;
        }

        public GrpcServiceProxyBuilder AddGrpcClient<TService>(string serviceName)
        {
            ClientDic.Add(typeof(TService), serviceName);
            return this;
        }
    }
}
