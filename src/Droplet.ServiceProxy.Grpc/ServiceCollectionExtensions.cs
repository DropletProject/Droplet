using Droplet.Discovery;
using Droplet.ServiceProxy.Grpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddGrpcServiceProxy(this IServiceCollection services, Action<GrpcServiceProxyBuilder> action = null)
        {
            var builder = new GrpcServiceProxyBuilder();
            action?.Invoke(builder);
            if (builder.CallInterceptorTypes != null && builder.CallInterceptorTypes.Count > 0)
            {
                var baseType = typeof(CallInterceptor);
                foreach (var type in builder.CallInterceptorTypes)
                {
                    services.AddSingleton(baseType, type);
                }
            }
            services.AddSingleton<IServiceProxy, ServiceProxy>();
            return services;
        }
    }
}
