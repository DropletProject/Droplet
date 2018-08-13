using Droplet.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

       

    }

    public class Test : AggregateRoot, IEntity
    {
        protected Test() { }
        public Test(string name)
        {
            Name = name;
            this.DomainEvents.Add(new CreateNewTestEvent( Name));
        }
        public string Id { get; set; }
        public string Name { get; set; }
      
    }

    public class TestWithId:Entity<int>
    {
        public TestWithId() { }
      
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
