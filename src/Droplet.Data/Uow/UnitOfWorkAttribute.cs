using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Data.Uow
{
    public class UnitOfWorkAttribute: Attribute
    {
        public UnitOfWorkAttribute(bool isTransactional = false)
        {
            IsTransactional = isTransactional;
        }
        public bool IsTransactional { get;  set; }
    }
}
