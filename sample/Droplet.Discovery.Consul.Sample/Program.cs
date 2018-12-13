using System;

namespace Droplet.Discovery.Consul.Sample
{
    class Program
    {
        static string consulAddress = @"http://192.168.8.6:8500/";

        static void Main(string[] args)
        {
            RegisterAndGetServices();
            Console.WriteLine("Hello World!");
        }

        static void Register()
        {
            var consulClient = ConsulClientFactory.Create(new ConsulDiscoveryConfiguration() { Address = consulAddress });
            ConsulServiceRegistrar registrar = new ConsulServiceRegistrar(consulClient);
            var info = registrar.RegisterServiceAsync("testServiceName","v1.0","192.168.0.81",8080).Result;

            registrar.DeregisterServiceAsync(info.Id).Wait();
        }

        static void RegisterAndGetServices()
        {
            var consulClient = ConsulClientFactory.Create(new ConsulDiscoveryConfiguration() { Address = consulAddress });
            ConsulServiceRegistrar registrar = new ConsulServiceRegistrar(consulClient);
            var info = registrar.RegisterServiceAsync("testServiceName", "v1.0", "192.168.0.81", 8080).Result;
            var info2 = registrar.RegisterServiceAsync("testServiceName", "v1.1", "192.168.0.81", 8090).Result;
            var info3 = registrar.RegisterServiceAsync("testServiceName", "v1.0", "192.168.0.81", 10101).Result;

            ConsulServiceDiscovery  discovery = new ConsulServiceDiscovery(consulClient, new ConsulDiscoveryConfiguration() { Address = consulAddress });

            var services = discovery.GetServicesAsync("testServiceName").Result;

            var services2 = discovery.GetServicesAsync("testServiceName","v1.0").Result;

            var service = discovery.GetServiceAsync("testServiceName").Result;
        }
    }
}
