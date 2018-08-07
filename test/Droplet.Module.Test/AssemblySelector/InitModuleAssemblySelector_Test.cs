using Droplet.Module.AssemblySelector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplet.Module.Test.AssemblySelector
{
    [TestClass]
    public class InitModuleAssemblySelector_Test
    {
        [TestMethod]
        public void ShouldHaveOneAssembly()
        {
            var moduleFinder = new InitModuleAssemblySelector();
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var assemblies = moduleFinder.SelectModuleAssembly(allAssemblies);
            Assert.AreEqual(1, assemblies.Count);
        }
    }

    public class TestInitModule : IInitModule
    {
        public void Init()
        {
            throw new NotImplementedException();
        }
    }
}
