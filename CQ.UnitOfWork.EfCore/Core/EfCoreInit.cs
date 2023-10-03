using CQ.UnitOfWork;
using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.EfCore.Abstractions;
using CQ.UnitOfWork.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CQ.UnitOfWork.EfCore
{
    public static class EfCoreInit
    {
        public static void AddEfCoreContext<TContext>(
            this IServiceCollection services,
            EfCoreConfig config,
            LifeTime lifeTime = LifeTime.Scoped)
            where TContext : EfCoreContext
        {
            config.Assert();

            var actions = (DbContextOptionsBuilder optionsBuilder) =>
            {
                switch (config.Engine)
                {
                    case EfCoreDataBaseEngine.SQL:
                        {
                            optionsBuilder.UseSqlServer(config.DatabaseConnection.ConnectionString);
                            break;
                        }

                    case EfCoreDataBaseEngine.SQL_LITE:
                        {
                            optionsBuilder.UseSqlite(new SqliteConnection(config.DatabaseConnection.ConnectionString));
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException($"Engine {config.Engine} not supported");
                        }
                }

                if (config.UseDefaultQueryLogger)
                {
                    optionsBuilder.LogTo(Console.WriteLine);
                }
                else if (config.Logger is not null)
                {
                    optionsBuilder.LogTo(config.Logger);
                }
            };
            var lifeTimeDb = lifeTime == LifeTime.Scoped ? ServiceLifetime.Scoped : lifeTime == LifeTime.Transient ? ServiceLifetime.Transient : ServiceLifetime.Singleton;

            services.AddDbContext<TContext>(actions, lifeTimeDb);
            services.AddDbContext<EfCoreContext, TContext>(actions, (ServiceLifetime)lifeTime);
            services.AddService<IDatabaseContext, TContext>(lifeTime);
        }

        /// <summary>
        /// Use default ef core context
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static void AddEfCoreRepository<TEntity>(this IServiceCollection services, LifeTime lifeTime = LifeTime.Scoped) where TEntity : class
        {
            services.AddService<IRepository<TEntity>, EfCoreRepository<TEntity>>(lifeTime);
            services.AddService<IUnitRepository<TEntity>, EfCoreRepository<TEntity>>(lifeTime);
            services.AddService<IEfCoreRepository<TEntity>, EfCoreRepository<TEntity>>(lifeTime);
        }

        /// <summary>
        /// Use specific context. Usefull when are multiple ef core context defined.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static void AddEfCoreRepository<TEntity, TContext>(this IServiceCollection services, LifeTime lifeTime = LifeTime.Scoped) 
            where TEntity : class
            where TContext : EfCoreContext
        {
            var repositoryImplementation = (IServiceProvider serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<TContext>();

                return new EfCoreRepository<TEntity>(context);
            };

            services.AddService<IRepository<TEntity>>(repositoryImplementation, lifeTime);
            services.AddService<IUnitRepository<TEntity>>(repositoryImplementation, lifeTime);
            services.AddService<IEfCoreRepository<TEntity>>(repositoryImplementation, lifeTime);
        }

        /// <summary>
        /// Sets custom repository under repository interfaces.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static void AddCustomEfCoreRepository<TEntity, TRepository>(this IServiceCollection services, LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : EfCoreRepository<TEntity>
        {
            services.AddService<IRepository<TEntity>, TRepository>(lifeTime);
            services.AddService<IUnitRepository<TEntity>, TRepository>(lifeTime);
            services.AddService<IEfCoreRepository<TEntity>, TRepository>(lifeTime);
        }
    }
}
