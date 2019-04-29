using Droplet.GrpcHost.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplet.GrpcHost
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseGrpcServer(this IHostBuilder builder,Action<GrpcHostBuilder> action)
        {
            var grpcHostBuilder = new GrpcHostBuilder();
            action.Invoke(grpcHostBuilder);

            builder
                 .ConfigureHostConfiguration((config) =>
                 {
                     config.AddEnvironmentVariables();//读取.NETCORE环境变量
                 })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile($"{grpcHostBuilder.ConfigDir}/{grpcHostBuilder.ConfigName}.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"{grpcHostBuilder.ConfigDir}/{grpcHostBuilder.ConfigName}.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddConsulDiscovery(new Uri(hostContext.Configuration.GetSection(grpcHostBuilder.ConsulAddrSection).Value), cfg =>
                   {
                       cfg.UseCache(10);
                   });
                   services.AddSingleton<IScopeServiceProvider, DefaultScopeServiceProvider>();
                   services.AddSingleton<ServerInterceptor, ExceptionHandlerInterceptor>(p =>
                   {
                       return new ExceptionHandlerInterceptor(p.GetService<ILoggerFactory>(), grpcHostBuilder.DefaultCode);
                   });
                   services.AddSingleton<ServerInterceptor, AccessLogInterceptor>();
                   services.AddSingleton<ServerInterceptor, UnitOfWorkInterceptor>();
                   foreach (var interceptorType in grpcHostBuilder.InterceptorTypes)
                   {
                       services.AddTransient(typeof(ServerInterceptor), interceptorType);
                   }
                   services.AddSingleton<IGrpcHostOption>(
                       new GrpcHostOption(
                           grpcHostBuilder.ServiceName, grpcHostBuilder.GrpcServers, grpcHostBuilder.IpAddress, grpcHostBuilder.Port, grpcHostBuilder.ServiceVersion));
                   services.AddHostedService<GrpcHostService>();
               })
               .ConfigureLogging((hostContext, configLogging) =>
               {
                   configLogging.AddConsole();
               });

            return builder;
        }

        public static IHostBuilder UseStartup<TStartup>(this IHostBuilder builder) where TStartup : class
        {
            var startupType = typeof(TStartup);

            builder.ConfigureServices((hostContext, services) => {

                var oneParaConstr = startupType.GetConstructors().FirstOrDefault(p => p.GetParameters().Count() == 1);
                if (oneParaConstr != null)
                {
                    dynamic startup = oneParaConstr.Invoke(new object[] { hostContext.Configuration });
                    startup.ConfigureServices(services);
                    return;
                }

                var defaultConstr = startupType.GetConstructors().FirstOrDefault(p => p.GetParameters().Count() == 0);
                if (defaultConstr != null)
                {
                    dynamic startup = defaultConstr.Invoke(null);
                    startup.ConfigureServices(services);
                    return;
                }

            });
            

            return builder;
        }
    }
}
