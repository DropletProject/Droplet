using Consul;
using Droplet.Discovery.Consul;
using Droplet.Discovery.Selectors;
using Droplet.ServiceProxy.Grpc;
using Grpc.Core.Interceptors;
using Helloworld;
using Microsoft.Extensions.DependencyInjection;
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
            var services = new ServiceCollection();
            services.AddConsulDiscovery(consulAddress, b => b.UseCache(5))
                .AddGrpcServiceProxy(c => c.UseCallInterceptor<TestCallInterceptor>());

            var provider = services.BuildServiceProvider();
          
            var proxy = provider.GetService<IServiceProxy>();

            int total = 200000;
            var watch = Stopwatch.StartNew();

            //while (total > 0)
            //{
            //    Console.WriteLine("请输入姓名：");
            //    var name = Console.ReadLine();
            //    try
            //    {
            //        SendMessage(proxy, total.ToString());
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"异常   {ex.ToString()}");
            //    }
            //    total--;
            //}

            for (int index = 0; index < total; index++)
            {
                SendMessage(proxy,index.ToString());
            }

            //Parallel.For(0, total, index =>
            //{
            //    SendMessage(proxy, index.ToString());
            //});
            watch.Stop();
            Console.WriteLine($"发送消息{total}次完成,耗时{watch.ElapsedMilliseconds}ms");
            Console.WriteLine($"按任意键结束");
            Console.ReadKey();
        }

        static void SendMessage(IServiceProxy proxy,string message)
        {
            try
            {
                var client = proxy.CreateAsync<GreeterClient>(Greeter.Descriptor.FullName).Result;
                client.SayHello(new HelloRequest() { Name = message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常   {ex.ToString()}");
            }
        }
    }

    public class TestCallInterceptor : CallInterceptor
    {
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var watch = Stopwatch.StartNew();
            var response = base.BlockingUnaryCall(request, context, continuation);
            watch.Stop();
            Console.WriteLine($"调用服务:{context.Method.FullName},发送消息{request.ToString()}次完成,耗时{watch.ElapsedMilliseconds}ms");
            return response;
        }
    }
}
