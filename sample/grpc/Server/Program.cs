using Droplet.Discovery.Consul;
using Grpc.Core;
using Helloworld;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    internal class GreeterImpl : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            string message = $"Hello {request.Name}";
            Console.WriteLine($"{DateTime.Now}  {message}");
            return Task.FromResult(new HelloReply { Message = message });
        }
    }

    internal class Program
    {
        static Uri consulAddress = new Uri(@"http://192.168.8.6:8500/");

        public static void Main(string[] args)
        {
            Console.WriteLine("Press ENTER port");
            var portStr = Console.ReadLine();
            var port = int.Parse(portStr);
            string serviceHost = GetIpAddress("192.168");

            var consulClient = ConsulClientFactory.Create(new ConsulDiscoveryConfiguration() { Address = consulAddress });
            var serviceRegistrar = new ConsulServiceRegistrar(consulClient);
            var server = new Grpc.Core.Server
            {
                Services = { Greeter.BindService(new GreeterImpl()) },
                Ports = { new ServerPort(serviceHost, port, ServerCredentials.Insecure) }
            };


            server.Start();
            var info = serviceRegistrar.RegisterServiceAsync(Greeter.Descriptor.FullName,"v1.0", serviceHost, port).Result;
            Console.WriteLine($"{info.Name} service listening on port {info.Port}");
            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();

            server.ShutdownAsync().Wait();
        }

        private static string GetIpAddress(string ipAddressPrefix)
        {
            var address = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Select(p => p.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address));
            if (!string.IsNullOrWhiteSpace(ipAddressPrefix))
            {
                address = address.Where(p => p.Address.ToString().StartsWith(ipAddressPrefix));
            }
            return address.First().Address.ToString();
        }
    }
}
