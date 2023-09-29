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
            LifeCycle lifeCycle = LifeCycle.SCOPED)
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
            var lifeTime = lifeCycle == LifeCycle.SCOPED ? ServiceLifetime.Scoped : lifeCycle == LifeCycle.TRANSIENT ? ServiceLifetime.Transient : ServiceLifetime.Singleton;

            services.AddDbContext<TContext>(actions, lifeTime);
            services.AddDbContext<EfCoreContext, TContext>(actions, lifeTime);
            services.AddService<IDatabaseContext, TContext>(lifeCycle);
        }

        /// <summary>
        /// Use default ef core context
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeCycle"></param>
        public static void AddEfCoreRepository<TEntity>(this IServiceCollection services, LifeCycle lifeCycle = LifeCycle.SCOPED) where TEntity : class
        {
            services.AddService<IRepository<TEntity>, EfCoreRepository<TEntity>>(lifeCycle);
            services.AddService<IUnitRepository<TEntity>, EfCoreRepository<TEntity>>(lifeCycle);
            services.AddService<IEfCoreRepository<TEntity>, EfCoreRepository<TEntity>>(lifeCycle);
        }

        /// <summary>
        /// Use specific context. Usefull when are multiple ef core context defined.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeCycle"></param>
        public static void AddEfCoreRepository<TEntity, TContext>(this IServiceCollection services, LifeCycle lifeCycle = LifeCycle.SCOPED) 
            where TEntity : class
            where TContext : EfCoreContext
        {
            var repositoryImplementation = (IServiceProvider serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<TContext>();

                return new EfCoreRepository<TEntity>(context);
            };

            services.AddService<IRepository<TEntity>>(repositoryImplementation, lifeCycle);
            services.AddService<IUnitRepository<TEntity>>(repositoryImplementation, lifeCycle);
            services.AddService<IEfCoreRepository<TEntity>>(repositoryImplementation, lifeCycle);
        }

        /// <summary>
        /// Sets custom repository under repository interfaces.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeCycle"></param>
        public static void AddCustomEfCoreRepository<TEntity, TRepository>(this IServiceCollection services, LifeCycle lifeCycle = LifeCycle.SCOPED)
            where TEntity : class
            where TRepository : EfCoreRepository<TEntity>
        {
            services.AddService<IRepository<TEntity>, TRepository>(lifeCycle);
            services.AddService<IUnitRepository<TEntity>, TRepository>(lifeCycle);
            services.AddService<IEfCoreRepository<TEntity>, TRepository>(lifeCycle);
        }
    }
}
