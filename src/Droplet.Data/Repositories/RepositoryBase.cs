using Droplet.Data.Entities;
using Droplet.Data.Uow;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Droplet.Data.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {

        public abstract void Delete(TEntity entity);

        public virtual Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
            return Task.FromResult(0);
        }

        public abstract TEntity FirstOrDefault();

        public abstract TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        public abstract Task<TEntity> FirstOrDefaultAsync();

        public abstract Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        public abstract IQueryable<TEntity> GetAll();

        public abstract IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        public virtual Task<IQueryable<TEntity>> GetAllAsync()
        {
            return Task.FromResult(GetAll());
        }

        public virtual Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(GetAll(predicate));
        }

        public abstract void Insert(TEntity entity);

        public virtual Task InsertAsync(TEntity entity)
        {
            Insert(entity);
            return Task.FromResult(0);
        }

        public abstract void Update(TEntity entity);

        public virtual Task UpdateAsync(TEntity entity)
        {
            Update(entity);
            return Task.FromResult(0);
        }
    }

}
