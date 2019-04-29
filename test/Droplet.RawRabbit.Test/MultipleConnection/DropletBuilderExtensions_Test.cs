using Droplet.Bootstrapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Droplet.RawRabbit.MultipleConnection;
using RawRabbit.Configuration;
using System.Linq;
using RawRabbit;
using Droplet.RawRabbit.AutoSubscribe;
using System.Threading.Tasks;
using RawRabbit.Pipe;
using System.Threading;
using Droplet.RawRabbit.Test.Fake;

namespace Droplet.RawRabbit.Test.MultipleConnection
{
    [TestClass]
    public class DropletBuilderExtensions_Test
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "TestConfig")]
        public void UseMultipleConnectionRawRabbit_ConfigName_Exist_Test()
        {
            var services = new ServiceCollection();
            services.BootDroplet().UseMultipleConnectionRawRabbit(cb => {
                cb.AddRawRabbitConfiguration("TestConfig", new RawRabbitConfiguration());
                cb.AddRawRabbitConfiguration("TestConfig", new RawRabbitConfiguration());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Default")]
        public void UseMultipleConnectionRawRabbit_Default_Exist_Test()
        {
            var services = new ServiceCollection();
            services.BootDroplet().UseMultipleConnectionRawRabbit(cb => {
                cb.AddRawRabbitConfiguration("TestConfig", new RawRabbitConfiguration(),true);
                cb.AddRawRabbitConfiguration("TestConfig2", new RawRabbitConfiguration(), true);
            });
        }

        [TestMethod]
        public void UseMultipleConnectionRawRabbit_AddRawRabbitConfiguration_Success_Test()
        {
            var services = new ServiceCollection();
            services.BootDroplet(new BootOption() { KeyWord = "Droplet.RawRabbit.Test" })
                .UseMultipleConnectionRawRabbit(cb => {
                    cb.AddRawRabbitConfiguration("TestConfig", new RawRabbitConfiguration(), true);
                    cb.AddRawRabbitConfiguration("TestConfig2", new RawRabbitConfiguration());
                });
            services.AddSingleton<IBusClient, FakeBusClient>();
            Assert.IsTrue(services.Any(s => s.ServiceType == typeof(IBusClientFactory)));
            Assert.IsTrue(services.Any(s => s.ServiceType == typeof(IBusClient)));

        }

    }



   

    public class TestConfigMessage
    {

    }

    [Connection("TestConfig")]
    public class TestConfigConsumeAsync : IConsumeAsync<TestConfigMessage>
    {
        public Task ConsumeAsync(TestConfigMessage message)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TestConfigMessage2
    {

    }
    [Connection("TestConfig2")]
    public class TestConfig2ConsumeAsync : IConsumeAsync<TestConfigMessage2>
    {
        public Task ConsumeAsync(TestConfigMessage2 message)
        {
            throw new System.NotImplementedException();
        }
    }
}
