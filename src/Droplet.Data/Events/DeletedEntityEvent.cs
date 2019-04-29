using Droplet.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Data.Events
{
    public class DeletedEntityEvent<TEntity> : MediatR.INotification where TEntity : IEntity
    {
        public DeletedEntityEvent(TEntity entity)
        {
            Entity = entity;
        }
        public TEntity Entity { get; set; }
    }
}
