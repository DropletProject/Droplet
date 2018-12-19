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

namespace Droplet.GrpcHost
{
    public class GrpcHostService : IHostedService
    {
        private readonly IGrpcHostOption _grpcHostOption;
        private readonly IServiceProvider _serviceProvider;

        public GrpcHostService(IGrpcHostOption grpcHostOption, IServiceProvider serviceProvider)
        {
            _grpcHostOption = grpcHostOption;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Server server = new Server
            {
                Services = { _grpcHostOption.GrpcServer(_serviceProvider) },
                Ports = { new ServerPort(_grpcHostOption.IpAddress, _grpcHostOption.Port, ServerCredentials.Insecure) }
            };

            server.Start();
            await _serviceProvider.GetService<IServiceRegistrar>().RegisterServiceAsync(_grpcHostOption.ServiceName, "", _grpcHostOption.IpAddress, _grpcHostOption.Port);
            Console.WriteLine($"GrpcServer is listen on { _grpcHostOption.IpAddress }:{ _grpcHostOption.Port}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public interface IGrpcHostOption
    {
        string ServiceName { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }
        Func<IServiceProvider, ServerServiceDefinition> GrpcServer { get; set; }
    }

    public class GrpcHostOption : IGrpcHostOption
    {
        public string ServiceName { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public Func<IServiceProvider, ServerServiceDefinition> GrpcServer { get; set; }

        public GrpcHostOption(string serviceName, Func<IServiceProvider, ServerServiceDefinition> grpcServer, string ipAddress = "", int port = 0)
        {
            ServiceName = serviceName;
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = GetLocalIp();

            if (port == 0)
                port = GetAvailablePort();

            IpAddress = ipAddress;
            Port = port;
            GrpcServer = grpcServer;
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
