using Droplet.Bootstrapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using Droplet.RawRabbit.MultipleConnection;
using RawRabbit.Configuration;
using Droplet.RawRabbit.AutoSubscribe;
using System.Threading.Tasks;
using RawRabbit.Enrichers.Attributes;
using RawRabbit.Configuration.Exchange;
using System.Collections.Generic;
using RawRabbit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Droplet.RawRabbit.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            MultipleConnectionRawRabbit();
            Console.WriteLine("Hello World!");
        }

        static void MultipleConnectionRawRabbit()
        {

            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.BootDroplet(new BootOption() { KeyWord = "Droplet.RawRabbit.Sample" })
                        .UseMultipleConnectionRawRabbit(cb => {
                            cb.AddRawRabbitConfiguration("TestConfig", new RawRabbitConfiguration()
                            {
                                Hostnames = new List<string> { "192.168.8.6" },
                                Username = "broadcast",
                                Password = "123456",
                                Port = 5672,
                                VirtualHost = "Broadcast"
                            }, true);
                            cb.AddRawRabbitConfiguration("TestConfig2", new RawRabbitConfiguration()
                            {
                                Hostnames = new List<string> { "192.168.8.6" },
                                Username = "broadcast",
                                Password = "123456",
                                Port = 5672,
                                VirtualHost = "dotnet"
                            });
                        }, rb => {
                            rb.UseAutoSubscribe();
                        });
                }).Build();

            host.Start();

            host.Services.StartAutoSubscribe();

            var busClientFactory = host.Services.GetService<IBusClientFactory>();

            var busClient = busClientFactory.Create("TestConfig");
            busClient.PublishAsync(new TestConfigMessage());

            var busClient2 = busClientFactory.Create("TestConfig2");
            busClient2.PublishAsync(new TestConfigMessage2());

            var defaultBusClient = host.Services.GetService<IBusClient>();
            defaultBusClient.PublishAsync(new TestConfigMessage());

            host.Run();
        }
    }


    [Exchange(Name = "rawRabbitTestExchange", Type = ExchangeType.Topic)]
    [Queue(Name = "rawRabbitTestQueue")]
    [Routing(RoutingKey = "TestConfigMessage")]
    public class TestConfigMessage
    {

    }

    [Connection("TestConfig")]
    public class TestConfigConsumeAsync : IConsumeAsync<TestConfigMessage>
    {
        public Task ConsumeAsync(TestConfigMessage message)
        {
            return Task.FromResult(0);
        }
    }

    [Exchange(Name = "rawRabbitTestExchange", Type = ExchangeType.Topic)]
    [Queue(Name = "rawRabbitTestQueue")]
    [Routing(RoutingKey = "TestConfigMessage2")]
    public class TestConfigMessage2
    {

    }
    [Connection("TestConfig2")]
    public class TestConfig2ConsumeAsync : IConsumeAsync<TestConfigMessage2>
    {
        public Task ConsumeAsync(TestConfigMessage2 message)
        {
            return Task.FromResult(0);
        }
    }
}
