using Droplet.AutoDI.ServiceSelector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplet.AutoDI.Dotnet.Test.ServiceSelector
{
    [TestClass]
    public class PartitionServiceSelector_Test
    {
        [TestMethod]
        public void TestGetPartitionService()
        {
            var selector = new PartitionServiceSelector();
            var types = selector.SelectServices(typeof(TestThirdClass));

            Assert.AreEqual(2, types.Count());
            Assert.AreEqual("IThird", types.ToList()[0].Name);
            Assert.AreEqual("IThirdClass", types.ToList()[1].Name);
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
