using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Droplet.Module.Test
{
    [TestClass]
    public class ModuleManager_Test
    {
        [TestMethod]
        public void ShouldInit_NoException()
        {
            var moduleManager = new ModuleManager();
            moduleManager.Init();
        }

        [TestMethod]
        public void ShouldGetTestAndInitAssembly()
        {
            var moduleManager = new ModuleManager();
            moduleManager.Init();
            Assert.AreEqual(2, moduleManager.GetModuleAssemblies().Count);
        }
    }
}
