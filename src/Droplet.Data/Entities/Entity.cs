using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Data.Entities
{
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {

        public virtual TPrimaryKey Id { get; set; }
    }
}
