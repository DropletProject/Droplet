using Droplet.Discovery.Consul;
using Droplet.Discovery.Selectors;
using Droplet.ServiceProxy.Grpc;
using Helloworld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static Helloworld.Greeter;

namespace Client
{
    class Program
    {
        static Uri consulAddress = new Uri(@"http://192.168.8.6:8500/");

        static void Main(string[] args)
        {
            var config = new ConsulDiscoveryConfiguration() { Address = consulAddress ,UseCache = true,CacheRefreshInterval = 5};
            var consulClient = ConsulClientFactory.Create(new ConsulDiscoveryConfiguration() { Address = consulAddress });
            var serviceDiscovery = new ConsulServiceDiscovery(consulClient, config);
            var proxy = new ServiceProxy(serviceDiscovery, new PollingServiceSelector(), null);

            int total = 20000;
            var watch = Stopwatch.StartNew();
            //while(true)
            //{
            //    Console.WriteLine("请输入姓名：");
            //    var name = Console.ReadLine();
            //    try
            //    {
            //        var client = proxy.CreateAsync<GreeterClient>(Greeter.Descriptor.FullName).Result;
            //        Console.WriteLine($"发送消息：{name}");
            //        client.SayHello(new HelloRequest() { Name = name });
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"异常   {ex.ToString()}");
            //    }
            //}
            for (int index = 0; index < total; index++)
            {
                var client = proxy.CreateAsync<GreeterClient>(Greeter.Descriptor.FullName).Result;
                Console.WriteLine($"发送消息：{index}");
                client.SayHello(new HelloRequest() { Name = index.ToString() });
            }

            //Parallel.For(0, total, index => {
            //    var client = proxy.CreateAsync<GreeterClient>(Greeter.Descriptor.FullName).Result;
            //    Console.WriteLine($"发送消息：{index}");
            //    client.SayHello(new HelloRequest() { Name = index.ToString() });
            //});
            watch.Stop();
            Console.WriteLine($"发送消息{total}次完成,耗时{watch.ElapsedMilliseconds}ms");
            Console.WriteLine($"按任意键结束");
            Console.ReadKey();
        }
    }

    public class TestCallInterceptor : CallInterceptor
    {

    }
}
