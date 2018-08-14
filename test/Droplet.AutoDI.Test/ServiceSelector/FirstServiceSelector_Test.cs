using Droplet.AutoDI.ServiceSelector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplet.AutoDI.Dotnet.Test.ServiceSelector
{
    [TestClass]
    public class FirstServiceSelector_Test
    {
        [TestMethod]
        public void TestGetFirstService()
        {
            var selector = new FirstServiceSelector();
            var types = selector.SelectServices(typeof(TestFirstClass));

            Assert.AreEqual(1, types.Count());
            Assert.AreEqual("IFirst", types.First().Name);
        }

        public abstract class AbsClass { }
        public interface IFirst { } 
        public interface ISecond { }
        public interface IThird { }

        public class TestFirstClass : AbsClass, IFirst, ISecond, IThird
        {

        }
    }
}
