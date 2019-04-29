using Droplet.Bootstrapper;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Droplet.AutoDI.Test
{
    [TestClass]
    public class DropletBuilderExtensions_Test
    {
        [TestMethod]
        public void TestAutoDIBuilder_GenericType()
        {
            var registrar = A.Fake<IComponentRegistrar>();

            var builder = new AutoDIBuilder(registrar, new List<Assembly>() { Assembly.GetExecutingAssembly() });
            builder.RegisterByInterface(typeof(GenericType<>), new ComponentAttribute());
        }
    }

    public interface GenericType<T>
    {

    }

    public class ConcreteType : GenericType<string>
    {

    }
}
