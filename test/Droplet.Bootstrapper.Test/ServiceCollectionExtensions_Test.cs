using Droplet.AutoDI;
using Droplet.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Droplet.Bootstrapper.Test
{
    [TestClass]
    public class ServiceCollectionExtensions_Test
    {
        [TestMethod]
        public void TestBootDroplet()
        {
            var services = new ServiceCollection();
            services
                .BootDroplet(new BootOption { EntryAssembly = typeof(ServiceCollectionExtensions_Test).Assembly })
                .UseAutoDI();

            var sp = services.BuildServiceProvider();

            var moduleFinder = sp.GetService<IModuleFinder>();
            var modules = moduleFinder.GetModuleAssemblies();
            Assert.AreNotEqual(0, modules.Count);

            var tc = sp.GetService<TestClass>();
            Assert.IsNotNull(tc);
        }

        [Component(RegisterService = RegisterServiceType.Self)]
        public class TestClass
        {
            
        }
    }
}
