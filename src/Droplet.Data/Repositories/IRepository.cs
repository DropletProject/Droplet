using Droplet.Data.Entities;
using Droplet.Data.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Droplet.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// 读取列表
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// 异步读取列表
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<TEntity>> GetAllAsync();

        /// <summary>
        /// 根据条件读取列表
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 异步根据条件读取列表
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 插入一个实体
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Insert( TEntity entity);

        /// <summary>
        /// 异步插入一个实体
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Task InsertAsync( TEntity entity);


        /// <summary>
        /// 更新指定
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Update( TEntity entity);

        /// <summary>
        ///  异步更新指定
        /// </summary>
        /// <param name="entity">The entity.</param>
        Task UpdateAsync( TEntity entity);

        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// 异步删除指定实体
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Task DeleteAsync( TEntity entity);

    }


    public interface IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        TEntity Get(TPrimaryKey id);

        Task<TEntity> GetAsync(TPrimaryKey id);

        void Delete(TPrimaryKey id);

        Task DeleteAsync(TPrimaryKey id);
    }
}
