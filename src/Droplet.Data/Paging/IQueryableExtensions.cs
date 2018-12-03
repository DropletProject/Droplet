using Droplet.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Droplet.Data.Paging
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TSource> Page<TSource>(
          this IQueryable<TSource> source, Pageable query, out int count)
          where TSource : class
        {
            count = source.Count();
            if (string.IsNullOrWhiteSpace(query.OrderBy))
            {
                query.OrderBy = "Id";
            }
            return source.OrderBy(query.OrderBy, query.IsDesc).Skip(query.PageSize * (query.PageIndex - 1)).Take(query.PageSize);
        }

        public static IQueryable<TEntity> Page<TEntity, TPrimary>(this IQueryable<TEntity> source, Pageable query, PageableResult pageableResult) where TEntity : Entity<TPrimary>
        {
            pageableResult.TotalCount = source.Count();

            if (string.IsNullOrWhiteSpace(query.OrderBy))
            {
                query.OrderBy = "Id";
            }

            return source.OrderBy(query.OrderBy, query.IsDesc)
                .Skip(query.PageSize * (query.PageIndex - 1))
                .Take(query.PageSize);
        }

        public static PageableResult<TResult> Page<TSource, TResult>(
           this IQueryable<TSource> source, Pageable query, Expression<Func<TSource, TResult>> selector)
           where TSource : class
           where TResult : class
        {
            var count = source.Count();
            if (string.IsNullOrWhiteSpace(query.OrderBy))
            {
                query.OrderBy = "Id";
            }
            var items = source.OrderBy(query.OrderBy, query.IsDesc)
                .Skip(query.PageSize * (query.PageIndex - 1))
                .Take(query.PageSize)
                .Select(selector)
                .ToList();
            var pagedList = new PageableResult<TResult>(query.PageIndex, query.PageSize, count, items);
            return pagedList;
        }

        public static PageableResult<TSource> Page<TSource>(
            this IQueryable<TSource> source, Pageable query)
            where TSource : class
        {
            var count = source.Count();
            if(string.IsNullOrWhiteSpace(query.OrderBy))
            {
                query.OrderBy = "Id";
            }
            var items = source.OrderBy(query.OrderBy, query.IsDesc)
                .Skip(query.PageSize * (query.PageIndex - 1))
                .Take(query.PageSize)
                .ToList();
            var pagedList = new PageableResult<TSource>(query.PageIndex, query.PageSize, count, items);
            return pagedList;
        }

       

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName)
        {
            return OrderBy(queryable, propertyName, false);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName, bool desc)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression conversion = Expression.Convert(Expression.Property(param, propertyName), typeof(object));
            var keySelector = Expression.Lambda<Func<T, object>>(conversion, param);
            return desc ? Queryable.OrderByDescending(queryable, keySelector) : Queryable.OrderBy(queryable, keySelector);
        }
    }
}
