using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Droplet.Data.Uow
{
    public class UnitOfWorkAttribute: Attribute
    {
        public UnitOfWorkAttribute(bool isTransactional = false)
        {
            IsTransactional = isTransactional;
        }

        public UnitOfWorkAttribute(IsolationLevel isolationLevel)
        {
            IsTransactional = true;
            IsolationLevel = isolationLevel;
        }
        public bool IsTransactional { get;  set; }

        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadUncommitted;
    }
}
