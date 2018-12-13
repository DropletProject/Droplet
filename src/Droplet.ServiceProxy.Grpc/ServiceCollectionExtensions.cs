using Droplet.Discovery;
using Droplet.ServiceProxy.Grpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGrpcServiceProxy(this IServiceCollection services,Action<GrpcServiceProxyConfiguration> action = null)
        {
            var configuration = new GrpcServiceProxyConfiguration();
            action?.Invoke(configuration);
            if(configuration.CallInterceptorTypes != null && configuration.CallInterceptorTypes.Length > 0)
            {
                var baseType = typeof(CallInterceptor);
                foreach (var type in configuration.CallInterceptorTypes)
                {
                    services.AddSingleton(baseType, type);
                }
            }
            services.AddSingleton<IServiceProxy, ServiceProxy>();
            return services;
        }
    }
}
