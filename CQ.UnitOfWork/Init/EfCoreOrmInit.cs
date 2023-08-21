using CQ.UnitOfWork.Entities.DataAccessConfig;
using CQ.UnitOfWork.Entities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CQ.UnitOfWork.Core;
using CQ.UnitOfWork.Exceptions;
using CQ.UnitOfWork.Entities.Context;
using CQ.UnitOfWork.Core.EfCore;

namespace CQ.UnitOfWork.Init
{
    public static class EfCoreOrmInit
    {
        public static void AddEfCoreOrm<TContext>(this IServiceCollection services, LifeCycles lifeCycle, TContext context)
            where TContext : EfCoreContext
        {
            AssertContext(context);

            services.AddService<DbContext>(lifeCycle, context);

            services.AddEfCoreConnection(lifeCycle);
        }

        private static void AssertContext<TContext>(TContext context) where TContext : EfCoreContext
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            context.Assert();
        }

        public static void AddEfCoreConnection(this IServiceCollection services, LifeCycles lifeCycle)
        {
            // For ping
            services.AddService<IDatabaseConnection, EfCoreConnection>(lifeCycle);

            // For build specific orm to repository
            services.AddService<EfCoreConnection>(lifeCycle);
        }

        public static void AddEfCoreRepository<TEntity>(this IServiceCollection services, LifeCycles lifeCycle) 
            where TEntity : class
        {
            services.AddService<IRepository<TEntity>, EfCoreRepository<TEntity>>(lifeCycle);

            services.AddService<IEfCoreRepository<TEntity>, EfCoreRepository<TEntity>>(lifeCycle);
        }

        public static void AddEfCoreRepository<TEntity, TRepository>(this IServiceCollection services, LifeCycles lifeCycle)
            where TEntity : class
            where TRepository : EfCoreRepository<TEntity>
        {
            services.AddService<IRepository<TEntity>, TRepository>(lifeCycle);

            services.AddService<IEfCoreRepository<TEntity>, TRepository>(lifeCycle);
        }
    }
}
