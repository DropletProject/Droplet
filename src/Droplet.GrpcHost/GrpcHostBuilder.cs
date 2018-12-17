using Droplet.Discovery;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static Grpc.Core.Server;

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
            var config = new ConfigurationBuilder()
                .AddJsonFile($"{configDir}/{configName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configDir}/{configName}.{env}.json", optional: true, reloadOnChange: true)
                .Build();
            var container = new ServiceCollection();
            container.AddSingleton<IConfiguration>(config);
            container.AddConsulDiscovery(new Uri(config.GetSection(consulAddrSection).Value), cfg => {
                cfg.UseCache(10);
            });

            InitStartup(container, config);
            var sp = container.BuildServiceProvider();

            var complete = new ManualResetEventSlim(false);

            try
            {
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    complete.Set();
                };

                if (string.IsNullOrEmpty(ipAddress))
                    ipAddress = GetLocalIp();

                if (port == 0)
                    port = GetAvailablePort();

                Server server = new Server
                {
                    Services = { grpcServer(sp) },
                    Ports = { new ServerPort(ipAddress, port, ServerCredentials.Insecure) }
                };

                sp.GetService<IServiceRegistrar>().RegisterServiceAsync(serviceName, "", ipAddress, port).Wait();
                server.Start();
                Console.WriteLine($"GrpcServer is listen on { ipAddress }:{ port}");
                complete.Wait();
            }
            catch (Exception ex)
            {
                complete.Set();
            }
            Environment.Exit(0);
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

        private string GetLocalIp()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            return localIP;
        }

        private int GetAvailablePort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
