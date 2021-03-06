﻿using Droplet.Data.Entities;
using Droplet.Data.Events;
using Droplet.Data.Uow;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
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
        public event EventHandler Completed;


        private readonly IMediator _eventBus;
        private IDbContextTransaction _dbContextTransaction;

        public TContext Context { get; private set; }


        public EntityFrameworkCoreUnitOfWork(TContext context, IMediator eventBus)
        {
            _eventBus = eventBus;
            Context = context;
        }


        public void Begin(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            _dbContextTransaction = Context.Database.BeginTransaction(isolationLevel);
        }


        private async Task PublishDomainEventsAsync()
        {
            var domainEntities = Context.ChangeTracker.Entries();

            var domainEvents = new List<INotification>();

            foreach (var domainEntity in domainEntities)
            {
                if(domainEntity.Entity is IAggregateRoot aggregateRoot)
                {
                    if(aggregateRoot.DomainEvents != null && aggregateRoot.DomainEvents.Any())
                    {
                        domainEvents.AddRange(aggregateRoot.DomainEvents);
                        aggregateRoot.DomainEvents.Clear();
                    }
                }
            }

            CancellationToken cancellationToken = new CancellationToken();
            var tasks = domainEvents
                .Select(async (domainEvent) => {
                    await _eventBus.Publish(domainEvent, cancellationToken);
                });

            await Task.WhenAll(tasks);

        }

        private INotification CreateNotification(Type genericEventType, object entity)
        {
            var entityType = entity.GetType();
            var eventType = genericEventType.MakeGenericType(entityType);

            return (INotification)Activator.CreateInstance(eventType, new[] { entity });
        }

        public void Complete()
        {
            PublishDomainEventsAsync().Wait();
            Context.SaveChanges();
            _dbContextTransaction?.Commit();

            OnCompleted();
        }

        public async Task CompleteAsync()
        {
            await PublishDomainEventsAsync();
            await Context.SaveChangesAsync();
            _dbContextTransaction?.Commit();

            OnCompleted();
        }

        /// <summary>
        /// Called to trigger <see cref="Completed"/> event.
        /// </summary>
        protected virtual void OnCompleted()
        {
            if (Completed == null)
            {
                return;
            }
            Completed(this, EventArgs.Empty);
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
                    _dbContextTransaction?.Dispose();
                    Context.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
