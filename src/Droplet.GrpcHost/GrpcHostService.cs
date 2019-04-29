using Droplet.Discovery;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Grpc.Core.Interceptors;
using System.Linq;
using Droplet.GrpcHost.Interceptors;
using static Grpc.Core.Server;

namespace Droplet.GrpcHost
{
    public class GrpcHostService : IHostedService
    {
        private readonly IGrpcHostOption _grpcHostOption;
        private readonly IServiceProvider _serviceProvider;

        private ServiceInformation _serviceInformation;

        public GrpcHostService(IGrpcHostOption grpcHostOption, IServiceProvider serviceProvider)
        {
            _grpcHostOption = grpcHostOption;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var interceptors = _serviceProvider.GetService<IEnumerable<ServerInterceptor>>();
            Server server = new Server
            {
                Ports = { new ServerPort(_grpcHostOption.IpAddress, _grpcHostOption.Port, ServerCredentials.Insecure) }
            };
            foreach (var grpcserver in _grpcHostOption.GrpcServers)
            {
                server.Services.Add(grpcserver.Invoke(_serviceProvider).Intercept(interceptors.ToArray()));
            }
            server.Start();
            _serviceInformation = await _serviceProvider.GetService<IServiceRegistrar>().RegisterServiceAsync(_grpcHostOption.ServiceName, _grpcHostOption.ServiceVersion, _grpcHostOption.IpAddress, _grpcHostOption.Port);
            Console.WriteLine($"GrpcServer is listen on { _grpcHostOption.IpAddress }:{ _grpcHostOption.Port}");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if(_serviceInformation != null)
            {
                await _serviceProvider.GetService<IServiceRegistrar>().DeregisterServiceAsync(_serviceInformation.Id);
            }
        }
    }

    public interface IGrpcHostOption
    {
        string ServiceName { get; set; }
        string ServiceVersion { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }

        List<Func<IServiceProvider, ServerServiceDefinition>> GrpcServers { get; }
    }

    internal class GrpcHostOption : IGrpcHostOption
    {
        public string ServiceName { get; set; }
        public string ServiceVersion { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public List<Func<IServiceProvider, ServerServiceDefinition>> GrpcServers { get;  }

        public GrpcHostOption()
        {
            GrpcServers = new List<Func<IServiceProvider, ServerServiceDefinition>>();
        }

        public GrpcHostOption(string serviceName, List<Func<IServiceProvider, ServerServiceDefinition>> grpcServers, string ipAddress = "", int port = 0,string version = ""):this()
        {
            ServiceName = serviceName;
            ServiceVersion = version;
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = GetLocalIp();

            if (port == 0)
                port = GetAvailablePort();

            IpAddress = ipAddress;
            Port = port;
            GrpcServers = grpcServers;
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
