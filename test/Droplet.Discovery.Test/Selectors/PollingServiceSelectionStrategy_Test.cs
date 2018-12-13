using Droplet.Discovery.Selectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Discovery.Test.Selectors
{
    [TestClass]
    public class PollingServiceSelectionStrategy_Test
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullException_WhenServicesIsNull_Select_Test()
        {
            PollingServiceSelector polling = new PollingServiceSelector();
            polling.SelectAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullException_WhenServicesIsEmpty_Select_Test()
        {
            PollingServiceSelector polling = new PollingServiceSelector();
            polling.SelectAsync(new List<ServiceInformation>());
        }

        [TestMethod]
        public void Select_Test()
        {
            var services = new List<ServiceInformation>() {
                new ServiceInformation() { Host = "192.168.1.5",Port=80,Id = "testService.192.168.1.5.80",Name = "testService",Version="v1.0" },
                new ServiceInformation() { Host = "192.168.1.6",Port=80,Id = "testService.192.168.1.6.80",Name = "testService",Version="v1.0" },
                new ServiceInformation() { Host = "192.168.1.7",Port=80,Id = "testService.192.168.1.7.80",Name = "testService",Version="v1.0" },
            };
            PollingServiceSelector polling = new PollingServiceSelector();
            var service = polling.SelectAsync(services);
            Assert.AreEqual(service.Id, "testService.192.168.1.5.80");

            var service2 = polling.SelectAsync(services);
            Assert.AreEqual(service2.Id, "testService.192.168.1.6.80");

            var service3 = polling.SelectAsync(services);
            Assert.AreEqual(service3.Id, "testService.192.168.1.7.80");

            var service4 = polling.SelectAsync(services);
            Assert.AreEqual(service4.Id, "testService.192.168.1.5.80"); 
        }

        [TestMethod]
        public void Concurrent_Select_Test()
        {
            var services = new List<ServiceInformation>() {
                new ServiceInformation() { Host = "192.168.1.5",Port=80,Id = "testService2.192.168.1.5.80",Name = "testService2",Version="v1.0" },
                new ServiceInformation() { Host = "192.168.1.6",Port=80,Id = "testService2.192.168.1.6.80",Name = "testService2",Version="v1.0" },
                new ServiceInformation() { Host = "192.168.1.7",Port=80,Id = "testService2.192.168.1.7.80",Name = "testService2",Version="v1.0" },
            };
            var serviceArr = services.ToArray();
            PollingServiceSelector polling = new PollingServiceSelector();
            ServiceInformation service = null;
            int count = 100;
            int totalCount = 0;
            for (int i = 1; i < count; i++)
            {
                Parallel.For(0, i, index => {
                    service = polling.SelectAsync(services);
                });
                totalCount += i;
                Assert.AreEqual(service.Id, serviceArr[(totalCount - 1) % services.Count].Id);
            }
        }
    }
}
