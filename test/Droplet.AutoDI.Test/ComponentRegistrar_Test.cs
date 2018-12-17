using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace Droplet.AutoDI.Test
{
    [TestClass]
    public class ComponentRegistrar_Test
    {
        [TestMethod]
        public void TestRegisterComponent_First()
        {
            var register = A.Fake<IRegister>();
            A.CallTo(() => register.Register(A<Type>._, A<Type>._)).Invokes((ctx) =>
             {
                 if ((Type)ctx.Arguments[0] != typeof(TestFirstClass))
                     Assert.Fail("component type is not correct");

                 if ((Type)ctx.Arguments[1] != typeof(IFirst))
                     Assert.Fail("service type is not correct");
             });

            var componentRegistrar = new ComponentRegistrar(register);
            componentRegistrar.RegisterComponent(typeof(TestFirstClass));
        }

        [TestMethod]
        public void TestRegisterComponent_SelfAndPart()
        {
            var register = A.Fake<IRegister>();
            A.CallTo(() => register.Register(A<Type>._, A<Type>._)).Invokes((ctx) =>
            {
                if ((Type)ctx.Arguments[0] != typeof(TestSelfAndPartitionClass))
                    Assert.Fail("component type is not correct");

                var srvType = (Type)ctx.Arguments[1];
                if (srvType != typeof(TestSelfAndPartitionClass) && srvType != typeof(ITestSelfAndPartition))
                    Assert.Fail("service type is not correct");
            });

            var componentRegistrar = new ComponentRegistrar(register);
            componentRegistrar.RegisterComponent(typeof(TestSelfAndPartitionClass));
        }

        [TestMethod]
        public void TestRegisterComponent_GenericType()
        {
            var register = A.Fake<IRegister>();
            A.CallTo(() => register.Register(A<Type>._, A<Type>._)).Invokes((ctx) =>
            {
                if ((Type)ctx.Arguments[0] != typeof(TestGenericType<>))
                    Assert.Fail("component type is not correct");

                var srvType = (Type)ctx.Arguments[1];
                if (srvType != typeof(IGenericType<>))
                    Assert.Fail("service type is not correct");
            });

            var componentRegistrar = new ComponentRegistrar(register);
            componentRegistrar.RegisterComponent(typeof(TestGenericType<>));
        }

        [TestMethod]
        public void TestRegisterComponent_GenericType_Concret()
        {
            var register = A.Fake<IRegister>();
            A.CallTo(() => register.Register(A<Type>._, A<Type>._)).Invokes((ctx) =>
            {
                if ((Type)ctx.Arguments[0] != typeof(ConcretGenericType))
                    Assert.Fail("component type is not correct");

                var srvType = (Type)ctx.Arguments[1];
                if (srvType != typeof(IGenericType<string>))
                    Assert.Fail("service type is not correct");
            });

            var componentRegistrar = new ComponentRegistrar(register);
            componentRegistrar.RegisterComponent(typeof(ConcretGenericType));
        }

        [TestMethod]
        public void TestRegisterComponent_All()
        {
            var register = A.Fake<IRegister>();
            var callCount = 0;
            A.CallTo(() => register.Register(A<Type>._, A<Type>._)).Invokes((ctx) =>
            {
                if ((Type)ctx.Arguments[0] != typeof(TestAllClass))
                    Assert.Fail("component type is not correct");

                callCount++;
            });

            var componentRegistrar = new ComponentRegistrar(register);
            componentRegistrar.RegisterComponent(typeof(TestAllClass));

            Assert.AreEqual(3, callCount);
        }

        [TestMethod]
        public void TestRegisterAssembly()
        {
            var register = A.Fake<IRegister>();
            var callCount = 0;
            A.CallTo(() => register.Register(A<Type>._, A<Type>._)).Invokes((ctx) =>
            {
                callCount++;
            });

            var componentRegistrar = new ComponentRegistrar(register);
            componentRegistrar.RegisterAssembly(Assembly.GetExecutingAssembly());

            Assert.AreEqual(7, callCount);
        }
    }

    public interface IFirst { }
    public interface ISecond { }
    public interface IThird { }
    public interface ITestSelfAndPartition{ }
    public interface IGenericType<T> { }

    [Component]
    public class TestFirstClass : IFirst, ISecond, IThird
    {}

    [Component(RegisterService = RegisterServiceType.Self | RegisterServiceType.Partition)]
    public class TestSelfAndPartitionClass : IFirst, ISecond, IThird, ITestSelfAndPartition
    { }

    [Component(RegisterService = RegisterServiceType.All)]
    public class TestAllClass : IFirst, ISecond, IThird
    { }

    [Component]
    public class TestGenericType<T> : IGenericType<T> { }

    [Component]
    public class ConcretGenericType : IGenericType<string> { }
}
