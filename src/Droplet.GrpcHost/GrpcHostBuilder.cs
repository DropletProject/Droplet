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
        public string EnvVarName { get; set; }
        public string ConfigName { get; set; }
        public string ConfigDir { get; set; }

        public Func<IServiceProvider, ServerServiceDefinition> GrpcServer { get; set; }

        public Type StartupType { get; set; }

        public string IpAddress { get; set; }
        public int Port { get; set; }

        public IServiceProvider Container { get; set; }

        public GrpcHostBuilder()
        {
            EnvVarName = "ASPNETCORE_ENVIRONMENT";
            ConfigName = "appsettings";
            ConfigDir = "./configs";
        }

        public GrpcHostBuilder UseConfig(string configDir = "", string configName = "")
        {
            if(!string.IsNullOrEmpty(configDir))
                ConfigDir = configDir;

            if (!string.IsNullOrEmpty(configName))
                ConfigName = configName;

            return this;
        }

        public GrpcHostBuilder UserHost(string ip = "", int port = 0)
        {
            IpAddress = ip;
            Port = port;

            return this;
        }

        public GrpcHostBuilder UseGrpcServer(Func<IServiceProvider, ServerServiceDefinition> serverServiceDefinition)
        {
            GrpcServer = serverServiceDefinition;

            return this;
        }

        public GrpcHostBuilder Startup<T>()
        {
            StartupType = typeof(T);

            return this;
        }

        public void Build()
        {
            var env = Environment.GetEnvironmentVariable(EnvVarName);
            var config = new ConfigurationBuilder()
                .AddJsonFile($"{ConfigDir}/{ConfigName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigDir}/{ConfigName}.{env}.json", optional: true, reloadOnChange: true)
                .Build();
            var container = new ServiceCollection();
            container.AddSingleton<IConfiguration>(config);

            InitStartup(container);
            var sp = container.BuildServiceProvider();

            var complete = new ManualResetEventSlim(false);

            try
            {
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    complete.Set();
                };

                if (string.IsNullOrEmpty(IpAddress))
                    IpAddress = GetLocalIp();

                if (Port == 0)
                    Port = GetAvailablePort();

                Server server = new Server
                {
                    Services = { GrpcServer(sp) },
                    Ports = { new ServerPort(GetLocalIp(), Port, ServerCredentials.Insecure) }
                };
                server.Start();
                Console.WriteLine($"GrpcServer is listen on { IpAddress }:{ Port}");
                complete.Wait();
            }
            catch (Exception ex)
            {
                complete.Set();
            }
            Environment.Exit(0);
        }

        private void InitStartup(IServiceCollection services)
        {
            if (StartupType == null)
                return;

            dynamic startup = Activator.CreateInstance(StartupType);
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
