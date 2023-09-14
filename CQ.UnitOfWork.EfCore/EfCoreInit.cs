using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Abstractions.Extensions;
using CQ.UnitOfWork.EfCore.Abstractions;
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
            LifeCycle lifeCycle = LifeCycle.SCOPED) where TContext : EfCoreContext
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

            services.AddDbContext<EfCoreContext, TContext>(actions, lifeTime);
        }

        public static void AddEfCoreRepository<TEntity>(this IServiceCollection services, LifeCycle lifeCycle = LifeCycle.SCOPED) where TEntity : class
        {
            services.AddService<IRepository<TEntity>, EfCoreRepository<TEntity>>(lifeCycle);
            services.AddService<IEfCoreRepository<TEntity>, EfCoreRepository<TEntity>>(lifeCycle);
        }

        public static void AddEfCoreRepository<TEntity, TRepository>(this IServiceCollection services, LifeCycle lifeCycle = LifeCycle.SCOPED)
            where TEntity : class
            where TRepository : EfCoreRepository<TEntity>
        {
            services.AddService<IRepository<TEntity>, TRepository>(lifeCycle);
            services.AddService<IEfCoreRepository<TEntity>, TRepository>(lifeCycle);
        }

        public static void AddEfCoreRepository<TService, TEntity, TRepository>(this IServiceCollection services, LifeCycle lifeCycle = LifeCycle.SCOPED)
            where TService : class, IEfCoreRepository<TEntity>
            where TEntity : class
            where TRepository : EfCoreRepository<TEntity>, TService
        {
            services.AddService<IRepository<TEntity>, TRepository>(lifeCycle);
            services.AddService<IEfCoreRepository<TEntity>, TRepository>(lifeCycle);
            services.AddService<TService, TRepository>(lifeCycle);
        }
    }
}
