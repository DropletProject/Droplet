using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Droplet.AutoDI.Test
{
    [TestClass]
    public class DotnetRegister_Test
    {
        [TestMethod]
        public void TestRegisterComponent_Transient()
        {
            var services = new ServiceCollection();
            var reg = new DotnetRegister(services);

            reg.Register(typeof(TransientClass), typeof(ITestClass));
            var sp = services.BuildServiceProvider();
            var s = sp.GetService<ITestClass>();
            Assert.AreEqual("Demo", s.Demo());

            s.SetDemo("Test");
            s = sp.GetService<ITestClass>();
            Assert.AreEqual("Demo", s.Demo());
        }

        [TestMethod]
        public void TestRegisterComponent_Singleton()
        {
            var services = new ServiceCollection();
            var reg = new DotnetRegister(services);

            reg.Register(typeof(SingletonClass), typeof(ITestClass));
            var sp = services.BuildServiceProvider();
            var s = sp.GetService<ITestClass>();
            Assert.AreEqual("Demo", s.Demo());

            s.SetDemo("Test");
            s = sp.GetService<ITestClass>();
            Assert.AreEqual("Test", s.Demo());
        }

        [TestMethod]
        public void TestRegisterComponent_Generic()
        {
            var services = new ServiceCollection();
            var reg = new DotnetRegister(services);

            reg.Register(typeof(GenericClass<>), typeof(IGenericInterface<>));
            var sp = services.BuildServiceProvider();
            var s = sp.GetService<IGenericInterface<TransientClass>>();
            Assert.AreEqual("HelloWorld", s.Demo());
        }
    }


    public interface ITestClass
    {
        string Demo();
        void SetDemo(string demo);
    }

    public interface ITempInterface
    {
    }

    [Component(RegisterService = RegisterServiceType.Self | RegisterServiceType.First)]
    public class TransientClass : ITestClass, ITempInterface
    {
        private string _demo = "Demo";

        public string Demo()
        {
            return _demo;
        }

        public void SetDemo(string demo)
        {
            _demo = demo;
        }
    }

    [Component(LiftTime = LifetimeType.Singleton)]
    public class SingletonClass : ITestClass, ITempInterface
    {
        private string _demo = "Demo";

        public string Demo()
        {
            return _demo;
        }

        public void SetDemo(string demo)
        {
            _demo = demo;
        }
    }

    [Component(RegisterService = RegisterServiceType.All)]
    public class TestAllClass_Demo : ITestClass, ITempInterface
    {
        private string _demo = "Demo";

        public string Demo()
        {
            return "Demo";
        }

        public void SetDemo(string demo)
        {
            _demo = demo;
        }
    }

    public interface IGenericInterface<T>
    {
        string Demo();
    }

    [Component(RegisterService = RegisterServiceType.Self | RegisterServiceType.First)]
    public class GenericClass<T> : IGenericInterface<T>
    {
        public string Demo()
        {
            return "HelloWorld";
        }
    }
}
