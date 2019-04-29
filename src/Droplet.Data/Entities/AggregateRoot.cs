using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Droplet.Data.Entities
{
    public interface IAggregateRoot
    {
        ICollection<INotification> DomainEvents { get; }
    }

    public abstract class AggregateRoot<TPrimaryKey> : Entity<TPrimaryKey>, IAggregateRoot
    {
        public AggregateRoot()
        {
            DomainEvents = new Collection<INotification>();
        }

        [NotMapped]
        public ICollection<INotification> DomainEvents { get; }
    }

    public abstract class AggregateRoot : AggregateRoot<int>
    {

    }
}
