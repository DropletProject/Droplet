using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.ServiceProxy.Grpc
{
    public class GrpcServiceProxyConfiguration
    {
        /// <summary>
        /// 调用拦截器类型
        /// </summary>
        public Type[] CallInterceptorTypes { get; set; }
    }
}
