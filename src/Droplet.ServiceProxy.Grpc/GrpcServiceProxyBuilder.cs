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
    }
}
