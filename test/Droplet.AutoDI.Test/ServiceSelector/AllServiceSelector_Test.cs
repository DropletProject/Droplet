using Droplet.AutoDI.ServiceSelector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplet.AutoDI.Dotnet.Test.ServiceSelector
{
    [TestClass]
    public class AllServiceSelector_Test
    {
        [TestMethod]
        public void TestGetAllService()
        {
            var selector = new AllServiceSelector();
            var types = selector.SelectServices(typeof(TestThirdClass));

            Assert.AreEqual(5, types.Count());
        }

        public abstract class AbsClass { }
        public interface IFirst { }
        public interface ISecond { }
        public interface IThird { }
        public interface IThirdClass { }


        public class TestThirdClass : AbsClass, IFirst, ISecond, IThird, IThirdClass
        {

        }
    }
}
