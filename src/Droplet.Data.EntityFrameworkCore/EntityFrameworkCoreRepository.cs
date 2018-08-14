using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Droplet.Data.Repositories;
using Droplet.Data.Entities;
using Droplet.Data.Uow;
using System.Threading.Tasks;

namespace Droplet.Data.EntityFrameworkCore
{
    public class EntityFrameworkCoreRepository<TEntity, TContext> : RepositoryBase<TEntity> where TEntity : class, IEntity where TContext : DbContext
    {
        private readonly EntityFrameworkCoreUnitOfWork<TContext> _unitOfWork;


        public virtual DbSet<TEntity> Table { get { return _unitOfWork.GetTable<TEntity>(); } }


        public EntityFrameworkCoreRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (EntityFrameworkCoreUnitOfWork<TContext>)unitOfWork;
        }

        public override IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        public override IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.Where(predicate);
        }

        public override void Insert(TEntity entity)
        {
            Table.Add(entity);

        }
        public override void Delete(TEntity entity)
        {
            Table.Remove(entity);
        }

        public override void Update(TEntity entity)
        {
            Table.Update(entity);
        }
    }

    public class EntityFrameworkCoreRepository<TEntity, TPrimaryKey, TContext>
        : EntityFrameworkCoreRepository<TEntity, TContext>, IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey> where TContext : DbContext
    {

        public EntityFrameworkCoreRepository(IUnitOfWork unitOfWork):base(unitOfWork)
        {
        }
        public void Delete(TPrimaryKey id)
        {
            var entity = Get(id);
            Delete(entity);
        }

        public Task DeleteAsync(TPrimaryKey id)
        {
            Delete(id);
            return Task.FromResult(0);
        }

        public TEntity Get(TPrimaryKey id)
        {
            return GetAll().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return Task.FromResult(Get(id));
        }

        protected virtual Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
    }
}
