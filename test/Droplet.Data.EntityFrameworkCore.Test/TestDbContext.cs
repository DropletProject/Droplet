using Droplet.Data.Entities;
using Droplet.Data.Repositories;
using Droplet.Data.Uow;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Data.EntityFrameworkCore.Test
{
    public class TestDbContext : DbContext
    {
        public TestDbContext() : base()
        {
        }
        
        protected  override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("test");
        }


        public DbSet<Test> Tests { get; set; }

        public DbSet<TestWithId> TestWithIds { get; set; }

        public DbSet<TestChangeEvent> TestChangeEvents { get; set; }

    }

    public class Test : AggregateRoot<int>
    {
        protected Test() { }
        public Test(string name)
        {
            Name = name;
            this.DomainEvents.Add(new CreateNewTestEvent( Name));
        }
        public int Id { get; set; }
        public string Name { get; set; }
      
    }

    public class TestWithId:Entity<int>
    {
        public TestWithId() { }
      
        public string Name { get; set; }
    }

    public class TestChangeEvent : Entity<string>
    {
        public TestChangeEvent() { }

        public string Name { get; set; }
    }

    public class CreateNewTestEvent : INotification
    {
        public CreateNewTestEvent( string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
   

}
