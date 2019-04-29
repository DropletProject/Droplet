using Droplet.Bootstrapper;
using Droplet.Data.Repositories;
using Droplet.Data.Uow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Data.EntityFrameworkCore.Test
{
    [TestClass]
    public class EntityFrameworkCoreBuilderExtensions_Test
    {
        [TestMethod]
        public void TestAddEntityFrameworkCore()
        {
            var services = new ServiceCollection();
            var builder = new DropletBuilder(services, new List<System.Reflection.Assembly>() { typeof(EntityFrameworkCoreBuilderExtensions_Test).Assembly });

            builder.UseEntityFrameworkCore<TestDbContext>(null);

            var serviceProvider = services.BuildServiceProvider();

            var testRepository = serviceProvider.GetService<IRepository<Test>>();
            Assert.IsNotNull(testRepository);
            Assert.AreEqual(testRepository.GetType(), typeof(EntityFrameworkCoreRepository<Test, TestDbContext>));

            var testRepository2 = serviceProvider.GetService<IRepository<Test,int>>();
            Assert.IsNotNull(testRepository2);
            Assert.AreEqual(testRepository2.GetType(), typeof(EntityFrameworkCoreRepository<Test,int, TestDbContext>));

            var repository = serviceProvider.GetService<ITestWithIdRepository>();
            Assert.IsNotNull(repository);
            Assert.AreEqual(repository.GetType(), typeof(TestWithIdRepository));


            var repository2 = serviceProvider.GetService<IRepository<TestWithId, int>>();
            Assert.IsNotNull(repository2);
            Assert.AreEqual(repository2.GetType(), typeof(TestWithIdRepository));

            var repository3 = serviceProvider.GetService<IRepository<TestWithId>>();
            Assert.IsNotNull(repository3);
            Assert.AreEqual(repository3.GetType(), typeof(TestWithIdRepository));
        }
    }

    public interface ITestWithIdRepository: IRepository<TestWithId,int>
    {
        void Testaaa();
    }

    public class TestWithIdRepository: EntityFrameworkCoreRepository<TestWithId,int, TestDbContext>, ITestWithIdRepository
    {
        public TestWithIdRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public void Testaaa()
        {
            return;
        }
    }


}
