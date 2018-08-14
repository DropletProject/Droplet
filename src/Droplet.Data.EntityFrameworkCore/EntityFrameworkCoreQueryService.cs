using Droplet.Data.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Data.EntityFrameworkCore
{
    public class EntityFrameworkCoreQueryService<TContext> : IQueryService, IDisposable  where TContext : DbContext
    {
        private readonly TContext _context;

        public EntityFrameworkCoreQueryService(TContext context)
        {
            _context = context;
        }

        public IEnumerable<TAny> Query<TAny>(string query, object parameters = null) where TAny : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TAny>> QueryAsync<TAny>(string query, object parameters = null) where TAny : class
        {
            return Task.FromResult(Query<TAny>(query, parameters));
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
                    _context?.Dispose();
                }
            }
            _disposed = true;
        }

       
    }
}
