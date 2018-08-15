using Droplet.Module.AssemblySelector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplet.Module.Test.AssemblySelector
{
    [TestClass]
    public class SimiliarAssemblySelector_Test
    {
        [TestMethod]
        public void ShouldHave_TestModule()
        {
            var assemFinder = new SimiliarAssemblySelector();
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var assemblies = assemFinder.SelectModuleAssembly(allAssemblies);
            Assert.AreEqual(1, assemblies.Count);
        }

        [TestMethod]
        public void ShouldHave_Module()
        {
            var assemFinder = new SimiliarAssemblySelector(typeof(SimiliarAssemblySelector_Test).Assembly);
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var assemblies = assemFinder.SelectModuleAssembly(allAssemblies);
            Assert.AreEqual(2, assemblies.Count);
        }
    }
}
