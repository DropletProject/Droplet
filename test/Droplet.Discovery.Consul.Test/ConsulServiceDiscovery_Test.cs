using Consul;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Droplet.Discovery.Consul.Test
{
    [TestClass]
    public class ConsulServiceDiscovery_Test
    {
        [TestMethod]
        public async Task GetServicesAsync_NotUseCache_Test()
        {
            string serviceName = "test";

            var consulClient = A.Fake<IConsulClient>();
            var discovery = new ConsulServiceDiscovery(consulClient, false, 0);

            A.CallTo(() => consulClient.Health.Service(serviceName, "", true, default))
                .Returns(
                    Task.FromResult(
                        new QueryResult<ServiceEntry[]>(new QueryResult(), new ServiceEntry[] { }))
                        );

            var services = await discovery.GetServicesAsync("test");
            var services2 = await discovery.GetServicesAsync("test");

            A.CallTo(() => consulClient.Health.Service(serviceName, "", true, default)).MustHaveHappened(2, Times.Exactly);

        }

        [TestMethod]
        public async Task GetServicesAsync_UseCache_Test()
        {
            string serviceName = "test";

            var consulClient = A.Fake<IConsulClient>();
            var discovery = new ConsulServiceDiscovery(consulClient, true, 5);

            A.CallTo(() => consulClient.Health.Service(A<string>._, "", true, default))
                .Returns(
                    Task.FromResult(
                        new QueryResult<ServiceEntry[]>(new QueryResult(), new ServiceEntry[] { }))
                        );

            var services = await discovery.GetServicesAsync("test");
            var services2 = await discovery.GetServicesAsync("test");

            A.CallTo(() => consulClient.Health.Service(serviceName, "", true, default)).MustHaveHappenedOnceExactly();

            var services3 = await discovery.GetServicesAsync("test2");
            A.CallTo(() => consulClient.Health.Service("test2", "", true, default)).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public async Task GetServicesAsync_UseCache_Interval_Test()
        {
            string serviceName = "test";

            var consulClient = A.Fake<IConsulClient>();
            var discovery = new ConsulServiceDiscovery(consulClient, true, 1);

            A.CallTo(() => consulClient.Health.Service(serviceName, "", true, default))
                .Returns(
                    Task.FromResult(
                        new QueryResult<ServiceEntry[]>(new QueryResult(), new ServiceEntry[] { }))
                        );

            var services = await discovery.GetServicesAsync("test");
            var services2 = await discovery.GetServicesAsync("test");
            A.CallTo(() => consulClient.Health.Service(serviceName, "", true, default)).MustHaveHappenedOnceExactly();
            Thread.Sleep(1000);
            var services3 = await discovery.GetServicesAsync("test");
            A.CallTo(() => consulClient.Health.Service(serviceName, "", true, default)).MustHaveHappened(2,Times.Exactly);
        }
    }
}
