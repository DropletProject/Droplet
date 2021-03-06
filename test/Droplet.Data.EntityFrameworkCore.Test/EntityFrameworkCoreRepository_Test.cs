using Droplet.Data.Events;
using Droplet.Data.Uow;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Droplet.Data.EntityFrameworkCore.Test
{
    [TestClass]
    public class EntityFrameworkCoreRepository_Test
    {
        static TestDbContext _context;
        static EntityFrameworkCoreUnitOfWork<TestDbContext> _unitOfWork;
        static Mock<IMediator> _mediator;
        static EntityFrameworkCoreRepository_Test()
        {
            var testWithIds = new List<TestWithId>
            {
                new TestWithId { Id = 1, Name = "A"},
                new TestWithId { Id = 2, Name = "B"},
                new TestWithId { Id = 3, Name = "C"},
                new TestWithId { Id = 4, Name = "D"},
                new TestWithId { Id = 5, Name = "E"},
                new TestWithId { Id = 6, Name = "F"},
            };
            _context = new TestDbContext();
            _context.TestWithIds.AddRange(testWithIds);
            _context.SaveChanges();
            _mediator = new Mock<IMediator>();
            _unitOfWork = new EntityFrameworkCoreUnitOfWork<TestDbContext>(_context, _mediator.Object);
        }


        [TestMethod]
        public void ShouldNotNull_GetTest()
        {
            var repository = new EntityFrameworkCoreRepository<TestWithId, int, TestDbContext>(_unitOfWork);

            var entity = repository.Get(1);

            Assert.IsNotNull(entity);
        }

        [TestMethod]
        public void DeleteTest()
        {
            var repository = new EntityFrameworkCoreRepository<TestWithId, int, TestDbContext>(_unitOfWork);

            var entity = repository.Get(6);
            repository.Delete(entity);
            _unitOfWork.Complete();
            Assert.IsNull(_context.TestWithIds.FirstOrDefault(s=>s.Id == 6));
        }

        [TestMethod]
        public void UpdateTest()
        {
            var repository = new EntityFrameworkCoreRepository<TestWithId, int, TestDbContext>(_unitOfWork);

            var entity = repository.Get(5);
            entity.Name = "UpdateTest";
            _unitOfWork.Complete();
            Assert.IsTrue(_context.TestWithIds.FirstOrDefault(s => s.Id == 5).Name == "UpdateTest");
        }

        [TestMethod]
        public void ShouldCountEqual6_GetAllTest()
        {
            var repository = new EntityFrameworkCoreRepository<TestWithId, int, TestDbContext>(_unitOfWork);

            var list = repository.GetAll().ToList();

            Assert.IsNotNull(list);

            Assert.IsTrue(list.Count > 1);
        }


        [TestMethod]
        public void ShouldSavaInStore_InsertAndCompleteTest()
        {
            using (var context = new TestDbContext())
            {
                var mediator = new Mock<IMediator>();
                var unitOfWork = new EntityFrameworkCoreUnitOfWork<TestDbContext>(context, mediator.Object);
                var repository = new EntityFrameworkCoreRepository<Test, TestDbContext>(unitOfWork);
                var test = new Test("ShouldSavaInStore_InsertAndCompleteTest");
                repository.Insert(test);
                unitOfWork.Complete();
                Assert.IsNotNull(context.Tests.FirstOrDefault(s => s.Name == "ShouldSavaInStore_InsertAndCompleteTest"));
            }
        }

        [TestMethod]
        public void ShouldNotSavaInStore_InsertOnlyTest()
        {
            using (var context = new TestDbContext())
            {
                var mediator = new Mock<IMediator>();
                using (var unitOfWork = new EntityFrameworkCoreUnitOfWork<TestDbContext>(context, mediator.Object))
                {
                    var repository = new EntityFrameworkCoreRepository<Test, TestDbContext>(unitOfWork);
                    var test = new Test("ShouldNotSavaInStore_InsertOnlyTest");
                    repository.Insert(test);
                    Assert.IsNull(context.Tests.FirstOrDefault(s => s.Name == "ShouldNotSavaInStore_InsertOnlyTest"));
                }
            }
        }

        [TestMethod]
        public void ShouldPublishDomainEvents_InsertAndCompleteTest()
        {
            using (var context = new TestDbContext())
            {
                var mediator = new Mock<IMediator>();
                var unitOfWork = new EntityFrameworkCoreUnitOfWork<TestDbContext>(context, mediator.Object);
                var repository = new EntityFrameworkCoreRepository<Test, TestDbContext>(unitOfWork);
                var test = new Test("ShouldPublishDomainEvents_InsertAndCompleteTest");
                repository.Insert(test);
                unitOfWork.Complete();
                mediator.Verify(s => s.Publish<INotification>(It.IsAny<CreateNewTestEvent>(), It.IsAny<CancellationToken>()), Times.Once);
                Assert.AreEqual(test.DomainEvents.Count, 0);
            }
        }

        [TestMethod]
        public void ShouldPublishChangeEntityEvent_UpdateAndCompleteTest()
        {
            using (var context = new TestDbContext())
            {
                var mediator = new Mock<IMediator>();
                var unitOfWork = new EntityFrameworkCoreUnitOfWork<TestDbContext>(context, mediator.Object);
                var repository = new EntityFrameworkCoreRepository<TestChangeEvent, string, TestDbContext>(unitOfWork);

                var test = new TestChangeEvent() { Id = "ShouldPublishUpdateEntityEvent_UpdateAndCompleteTest" };
                repository.Insert(test);
                unitOfWork.Complete();
                mediator.Verify(s => s.Publish<INotification>(It.IsAny<CreatedEntityEvent<TestChangeEvent>>(), It.IsAny<CancellationToken>()), Times.Once);

                var test2 = repository.Get(test.Id);
                test2.Name = "121212";
                repository.Update(test2);
                unitOfWork.Complete();
                mediator.Verify(s => s.Publish<INotification>(It.IsAny<UpdatedEntityEvent<TestChangeEvent>>(), It.IsAny<CancellationToken>()), Times.Once);

                repository.Delete(test.Id);
                unitOfWork.Complete();
                mediator.Verify(s => s.Publish<INotification>(It.IsAny<DeletedEntityEvent<TestChangeEvent>>(), It.IsAny<CancellationToken>()), Times.Once);

            }
        }



    }



}
