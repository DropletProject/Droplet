using AutoMapper;
using Droplet.AutoMapper.Attributes;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace Droplet.AutoMapper.Test
{
    [TestClass]
    public class AutoMapperRegistrar_Test
    {
        [TestMethod]
        public void TestAutoCreateMap()
        {
            var services = new ServiceCollection();
            var registrar = new AutoMapperRegistrar(services, Assembly.GetExecutingAssembly());
            registrar.CreateMap();

            var sp = services.BuildServiceProvider();
            var mapper = sp.GetService<IMapper>();

            var ac = new AClass();
            ac.Field = "HelloWorld";
            var amc = mapper.Map<AMapClass>(ac);
            Assert.AreEqual(ac.Field, amc.Field);

            var bc = new BMapClass();
            bc.Field = "HelloWorld";
            bc.Field3 = "HelloWorld3";
            var bmc = mapper.Map<BClass>(bc);
            Assert.AreEqual(ac.Field, amc.Field);

            Assert.ThrowsException<AutoMapperConfigurationException>(() =>
            {
                mapper.Map<BMapClass>(bmc);
            });

            var cc = new CClass();
            cc.Field = "HelloWorld";
            cc.Field3 = "HelloWorld3";
            var cmc = mapper.Map<CMapClass>(cc);
            Assert.AreEqual(ac.Field, amc.Field);

            Assert.ThrowsException<AutoMapperConfigurationException>(() =>
            {
                mapper.Map<CClass>(cmc);
            });
        }
    }

    [Map(typeof(AMapClass))]
    public class AClass
    {
        public string Field { get; set; }
        public string Field3 { get; set; }
    }

    public class AMapClass
    {
        public string Field { get; set; }
        public string Field2 { get; set; }
    }

    [MapFrom(typeof(BMapClass))]
    public class BClass
    {
        public string Field { get; set; }
        public string Field2 { get; set; }
    }

    public class BMapClass
    {
        public string Field { get; set; }
        public string Field3 { get; set; }
    }

    [MapTo(typeof(CMapClass))]
    public class CClass
    {
        public string Field { get; set; }
        public string Field3 { get; set; }
    }

    public class CMapClass
    {
        public string Field { get; set; }
        public string Field2 { get; set; }
    }
}
