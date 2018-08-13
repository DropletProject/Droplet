using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Droplet.Data.Entities
{
    public abstract class AggregateRoot
    {
        public AggregateRoot()
        {
            DomainEvents = new Collection<INotification>();
        }

        [NotMapped]
        public ICollection<INotification> DomainEvents { get; }

    }
}
