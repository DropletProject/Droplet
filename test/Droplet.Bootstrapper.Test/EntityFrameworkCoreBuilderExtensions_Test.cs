using Droplet.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Bootstrapper.Test
{
    [TestClass]
    public class EntityFrameworkCoreBuilderExtensions_Test
    {
        [TestMethod]
        public void TestAddEntityFrameworkCore()
        {
            var services = new ServiceCollection();
            var builder = new DropletBuilder(services, new List<System.Reflection.Assembly>() { typeof(EntityFrameworkCoreBuilderExtensions_Test).Assembly });

            builder.AddEntityFrameworkCore<TestDbContext>(null);

            var serviceProvider = services.BuildServiceProvider();

            var testRepository = serviceProvider.GetService<IRepository<Test>>();
            Assert.IsNotNull(testRepository);
            Assert.AreEqual(testRepository.GetType(), typeof(TestRepository));
            Assert.IsNotNull(serviceProvider.GetService<IRepository<TestWithId,int>>());
            Assert.IsNotNull(serviceProvider.GetService<INullRepository>());
        }
    }


}
