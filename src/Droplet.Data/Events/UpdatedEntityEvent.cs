using System;
using System.Collections.Generic;
using System.Text;
using Droplet.Data.Entities;

namespace Droplet.Data.Events
{
    public class UpdatedEntityEvent<TEntity>:MediatR.INotification where TEntity : IEntity
    {
        public UpdatedEntityEvent(TEntity entity)
        {
            Entity = entity;
        }
        public TEntity Entity { get; set; }
    }
}
