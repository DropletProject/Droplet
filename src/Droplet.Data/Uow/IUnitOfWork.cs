using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Data.Uow
{
    public interface IUnitOfWork : IDisposable
    {

        void Begin(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);

        void Complete();

        Task CompleteAsync();

    }
}
