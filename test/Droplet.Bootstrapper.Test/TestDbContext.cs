using Droplet.Data.Entities;
using Droplet.Data.EntityFrameworkCore;
using Droplet.Data.Repositories;
using Droplet.Data.Uow;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Bootstrapper.Test
{
    public class TestDbContext : DbContext
    {
        public TestDbContext() : base()
        {
        }
        
        public DbSet<Test> Tests { get; set; }

        public DbSet<TestWithId> TestWithIds { get; set; }

       

    }

    public class Test : AggregateRoot, IEntity
    {
        protected Test() { }
        public Test(string name)
        {
            Name = name;
        }
        public string Id { get; set; }
        public string Name { get; set; }
      
    }

    public class TestWithId:Entity<int>
    {
        public TestWithId() { }
      
        public string Name { get; set; }
    }



    public class TestRepository : EntityFrameworkCoreRepository<Test, TestDbContext>, IRepository<Test>
    {
        public TestRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public override void Insert(Test entity)
        {
            base.Insert(entity);
        }
    }


    public interface INullRepository : IRepository
    {
        Task Nothing();
    }

    public class NullRepository : INullRepository
    {
        public async Task Nothing()
        {
            await Task.FromResult(0);
        }
    }

}
