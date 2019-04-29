using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Droplet.Utility.Extensions;

namespace Droplet.Utility.Test.Extensions
{
    [TestClass]
    public class ObjectExtensions_Test
    {
        [TestMethod]
        public void TestGetEnumDescription()
        {
            var hello = TestEnum.Hello;
            Assert.AreEqual("Hello", hello.GetDescription());

            var world = TestEnum.World;
            Assert.AreEqual("World", world.GetDescription());

            var ruok = TestEnum.AreYouOk;
            Assert.AreEqual("AreYouOk", ruok.GetDescription());
        }

        [TestMethod]
        public void TestGetClassDescription()
        {
            var testClass = new TestClass();
            Assert.AreEqual("TestClass", testClass.GetDescription());
        }

        [TestMethod]
        public void TestChildClassDescription()
        {
            var childClass = new ChildClass();
            Assert.AreEqual("TestClass", childClass.GetDescription(true));

            Assert.AreEqual("", childClass.GetDescription(false));
        }
    }

    public enum TestEnum
    {
        [System.ComponentModel.Description("Hello")]
        Hello,
        [System.ComponentModel.Description("World")]
        World,
        [System.ComponentModel.Description("AreYouOk")]
        AreYouOk
    }

    [System.ComponentModel.Description("TestClass")]
    public class TestClass
    {

    }

    public class ChildClass : TestClass
    {

    }
}
