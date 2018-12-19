using Droplet.Discovery;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static Grpc.Core.Server;
using Microsoft.Extensions.Logging;

namespace Droplet.GrpcHost
{
    public static class GrpcHost
    {
        public static GrpcHostBuilder CreateBuilder()
        {
            return new GrpcHostBuilder();
        }
    }

    public class GrpcHostBuilder
    {
        public string serviceName { get; set; }
        public string envVarName { get; set; }
        public string configName { get; set; }
        public string configDir { get; set; }

        public Func<IServiceProvider, ServerServiceDefinition> grpcServer { get; set; }

        public Type startupType { get; set; }

        public string ipAddress { get; set; }
        public int port { get; set; }

        public IServiceProvider container { get; set; }
        public string consulAddrSection { get; set; }

        public GrpcHostBuilder()
        {
            envVarName = "ASPNETCORE_ENVIRONMENT";
            configName = "appsettings";
            configDir = "./configs";
            consulAddrSection = "consulAddr";
        }

        public GrpcHostBuilder ServiceName(string name)
        {
            serviceName = name;

            return this;
        }

        public GrpcHostBuilder Config(string configDir = "", string configName = "")
        {
            if(!string.IsNullOrEmpty(configDir))
                this.configDir = configDir;

            if (!string.IsNullOrEmpty(configName))
                this.configName = configName;

            return this;
        }

        public GrpcHostBuilder Host(string ip = "", int port = 0)
        {
            ipAddress = ip;
            this.port = port;

            return this;
        }

        public GrpcHostBuilder GrpcServer(Func<IServiceProvider, ServerServiceDefinition> serverServiceDefinition)
        {
            grpcServer = serverServiceDefinition;

            return this;
        }

        public GrpcHostBuilder ConsulAddrSetion(string consulAddrSetion)
        {
            consulAddrSection = consulAddrSetion;

            return this;
        }

        public GrpcHostBuilder Startup<T>()
        {
            startupType = typeof(T);

            return this;
        }

        public void Build()
        {
            CheckParameter();

            var env = Environment.GetEnvironmentVariable(envVarName);

            var host = new HostBuilder()
                .ConfigureAppConfiguration(cfg => {
                    cfg
                        .AddJsonFile($"{configDir}/{configName}.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"{configDir}/{configName}.{env}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((ctx, srv) => {
                    
                    srv.AddConsulDiscovery(new Uri(ctx.Configuration.GetSection(consulAddrSection).Value), cfg => {
                        cfg.UseCache(10);
                    });
                    InitStartup(srv, ctx.Configuration);
                    InitGrpcServer(srv);
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                })
                .Build();

            
            host.Run();
        }

        private void InitGrpcServer(IServiceCollection srv)
        {
            srv.AddSingleton<IGrpcHostOption>(new GrpcHostOption(serviceName, grpcServer, ipAddress, port));
            srv.AddHostedService<GrpcHostService>();
        }

        private void CheckParameter()
        {
            if (string.IsNullOrEmpty(serviceName))
                throw new ArgumentException("parameter can not be empty", nameof(serviceName));
        }

        private void InitStartup(IServiceCollection services, IConfiguration configuration)
        {
            if (startupType == null)
                return;

            dynamic startup = Activator.CreateInstance(startupType, configuration);
            startup.ConfigureServices(services);
        }
    }
}
