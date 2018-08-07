using Droplet.Module.AssemblySelector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplet.Module.Test.AssemblySelector
{
    [TestClass]
    public class KeywordAssemblySelector_Test
    {
        [TestMethod]
        public void ShouldHave_NoModule()
        {
            var assemFinder = new KeywordAssemblySelector("TestModule");
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var assemblies = assemFinder.SelectModuleAssembly(allAssemblies);
            Assert.AreEqual(0, assemblies.Count);
        }

        [TestMethod]
        public void ShouldHave_TestAndSrcModule()
        {
            var assemFinder = new KeywordAssemblySelector("Droplet.");
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var assemblies = assemFinder.SelectModuleAssembly(allAssemblies);
            Assert.AreEqual(2, assemblies.Count);
        }
    }
}
