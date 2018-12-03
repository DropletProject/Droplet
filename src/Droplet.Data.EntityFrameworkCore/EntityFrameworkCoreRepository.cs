﻿using System;
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
        protected readonly EntityFrameworkCoreUnitOfWork<TContext> _unitOfWork;


        public virtual DbSet<TEntity> Table { get { return _unitOfWork.Context.Set<TEntity>(); } }


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
            return GetAll().Where(predicate);
        }

        public override void Insert(TEntity entity)
        {
            Table.Add(entity);
        }
        public override void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
        }

        public override void Update(TEntity entity)
        {
            AttachIfNot(entity);
            _unitOfWork.Context.Entry(entity).State = EntityState.Modified;
            Table.Update(entity);
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = _unitOfWork.Context.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }

            Table.Attach(entity);
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

        public virtual TEntity Get(TPrimaryKey id)
        {
            var entity = GetAll().FirstOrDefault(CreateEqualityExpressionForId(id));
            if(entity == null)
            {
                throw new ArgumentException($"{typeof(TEntity).Name}不存在Id={id}的记录");
            }
            return entity;
        }

        public virtual Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return Task.FromResult(Get(id));
        }

        public virtual TPrimaryKey InsertAndGetId(TEntity entity)
        {
            Insert(entity);
            if(entity.IsTransient())
            {
                _unitOfWork.Context.SaveChanges();
            }
            return entity.Id;
        }

        public virtual async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            await InsertAsync(entity);
            if (entity.IsTransient())
            {
                await _unitOfWork.Context.SaveChangesAsync();
            }
            return entity.Id;
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
