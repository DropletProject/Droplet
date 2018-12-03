using Droplet.Data.Entities;
using Droplet.Data.EntityFrameworkCore;
using Droplet.Data.Repositories;
using Droplet.Data.Uow;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Droplet.Bootstrapper
{
    public static  class EntityFrameworkCoreBuilderExtensions
    {
        public static DropletBuilder AddEntityFrameworkCore<TContext>(this DropletBuilder builder, Action<DbContextOptionsBuilder> optionsAction = null) where TContext : DbContext
        {
            builder.AddMediator();
            builder.ServiceCollection.AddDbContext<TContext>(optionsAction);
            builder.ServiceCollection.AddScoped(typeof(IUnitOfWork), typeof(EntityFrameworkCoreUnitOfWork<TContext>));
            builder.ServiceCollection.RegisterGenericRepositoriesAndMatchDbContexes(typeof(TContext));

            builder.ServiceCollection.Scan(scan =>
               scan.FromAssemblies(builder.RegisterAssemblys)
               .AddClasses(classes =>
                   classes.AssignableTo(typeof(IRepository)))
               .AsImplementedInterfaces().WithTransientLifetime()
           );

            return builder;
        }

        private static void AddMediator(this DropletBuilder builder)
        {
            builder.ServiceCollection.AddScoped<ServiceFactory>(p => p.GetService);
            builder.ServiceCollection.AddScoped<IMediator, Mediator>();
            builder.ServiceCollection.AddMediatR(builder.RegisterAssemblys);
        }


        private static void RegisterGenericRepositoriesAndMatchDbContexes(this IServiceCollection services, Type dbContextType)
        {
            var list = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var entityTypes =
                from property in list
                where
                    IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>)) &&
                    typeof(IEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
                select property.PropertyType.GenericTypeArguments[0];

            foreach (var entityType in entityTypes)
            {
                if (IsAssignableToGenericType(entityType, typeof(Entity<>)))
                {
                    services.AddTransient(typeof(IRepository<,>).MakeGenericType(entityType, GetPrimaryKeyType(entityType)),
                        typeof(EntityFrameworkCoreRepository<,,>).MakeGenericType(entityType, GetPrimaryKeyType(entityType), dbContextType));
                }
                ;
                services.AddTransient(typeof(IRepository<>).MakeGenericType(entityType),
                    typeof(EntityFrameworkCoreRepository<,>).MakeGenericType(entityType, dbContextType));
            }
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var givenTypeInfo = givenType.GetTypeInfo();

            if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            foreach (var interfaceType in givenType.GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenTypeInfo.BaseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(givenTypeInfo.BaseType, genericType);
        }

        private static Type GetPrimaryKeyType(Type entityType)
        {
            foreach (var interfaceType in entityType.GetTypeInfo().GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEntity<>))
                {
                    return interfaceType.GenericTypeArguments[0];
                }
            }

            throw new Exception("Can not find primary key type of given entity type: " + entityType + ". Be sure that this entity type implements IEntity<TPrimaryKey> interface");
        }
    }
}
