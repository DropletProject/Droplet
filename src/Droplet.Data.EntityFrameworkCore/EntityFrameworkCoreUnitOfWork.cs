using Droplet.Data.Entities;
using Droplet.Data.Uow;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Droplet.Data.EntityFrameworkCore
{
    public class EntityFrameworkCoreUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly IMediator _eventBus;
        private readonly TContext _context;

        public EntityFrameworkCoreUnitOfWork(TContext context, IMediator eventBus)
        {
            _eventBus = eventBus;
            _context = context;
        }

        public DbSet<TEntity> GetTable<TEntity>() where TEntity :class
        {
            return _context.Set<TEntity>();
        }


        public void Begin(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
        }

        private async Task PublishDomainEventsAsync()
        {
            var domainEntities = _context.ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            CancellationToken cancellationToken = new CancellationToken();
            var tasks = domainEvents
                .Select(async (domainEvent) => {
                    await _eventBus.Publish(domainEvent, cancellationToken);
                });

            await Task.WhenAll(tasks);

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.DomainEvents.Clear());
        }

        public void Complete()
        {
            PublishDomainEventsAsync().Wait();
            _context.SaveChanges();
        }

        public async Task CompleteAsync()
        {
            await PublishDomainEventsAsync();
            await _context.SaveChangesAsync();
        }


        private bool _disposed = false;
      
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

       
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
