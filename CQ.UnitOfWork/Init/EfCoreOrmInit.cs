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
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;

namespace CQ.UnitOfWork.Init
{
    public static class EfCoreOrmInit
    {
        /// <summary>
        /// Use AddDbContext by EfCore. TContext need a constructor with DbContextOptions and pass it to parent class.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="lifeCycle"></param>
        public static void AddDbEfCoreOrm<TContext>(this IServiceCollection services, EfCoreConfig config, LifeCycles lifeCycle = LifeCycles.SCOPED)
            where TContext : EfCoreContext
        {
            AssertConfig(config);

            var actions = (DbContextOptionsBuilder optionsBuilder) =>
            {
                if (EfCoreDataBaseEngines.SQL == config.Engine)
                {
                    optionsBuilder.UseSqlServer(config.DatabaseConnection.ConnectionString);
                }

                if (EfCoreDataBaseEngines.SQL_LITE == config.Engine)
                {
                    optionsBuilder.UseSqlite(new SqliteConnection(config.DatabaseConnection.ConnectionString));
                }

                if (config.UseDefaultQueryLogger)
                {
                    optionsBuilder.LogTo(Console.WriteLine);
                }

                if (config.Logger is not null)
                {
                    optionsBuilder.LogTo(config.Logger);
                }
            };
            var lifeTime = lifeCycle == LifeCycles.SCOPED ? ServiceLifetime.Scoped : lifeCycle == LifeCycles.TRANSIENT ? ServiceLifetime.Transient : ServiceLifetime.Singleton;
            services.AddDbContext<DbContext, TContext>(actions, lifeTime);

            services.AddEfCoreConnection(lifeCycle);
        }

        public static void AddEfCoreOrm<TContext>(this IServiceCollection services, EfCoreConfig config, LifeCycles lifeCycle = LifeCycles.SCOPED)
            where TContext : EfCoreContext
        {
            AssertConfig(config);

            services.AddService(config, LifeCycles.SINGLETON);

            services.AddService<DbContext, TContext>(lifeCycle);

            services.AddEfCoreConnection(lifeCycle);
        }

        private static void AssertConfig(EfCoreConfig config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            config.Assert();
        }

        public static void AddEfCoreConnection(this IServiceCollection services, LifeCycles lifeCycle = LifeCycles.SCOPED)
        {
            // For ping
            services.AddService<IDatabaseConnection, EfCoreConnection>(lifeCycle);

            // For build specific orm to repository
            services.AddService<EfCoreConnection>(lifeCycle);
        }

        public static void AddEfCoreRepository<TEntity>(this IServiceCollection services, LifeCycles lifeCycle = LifeCycles.SCOPED)
            where TEntity : class
        {
            services.AddService<IRepository<TEntity>, EfCoreRepository<TEntity>>(lifeCycle);

            services.AddService<IEfCoreRepository<TEntity>, EfCoreRepository<TEntity>>(lifeCycle);
        }

        public static void AddEfCoreRepository<TEntity, TRepository>(this IServiceCollection services, LifeCycles lifeCycle = LifeCycles.SCOPED)
            where TEntity : class
            where TRepository : EfCoreRepository<TEntity>
        {
            services.AddService<IRepository<TEntity>, TRepository>(lifeCycle);

            services.AddService<IEfCoreRepository<TEntity>, TRepository>(lifeCycle);
        }
    }
}
