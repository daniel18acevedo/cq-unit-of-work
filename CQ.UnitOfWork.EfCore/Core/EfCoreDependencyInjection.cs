using CQ.UnitOfWork;
using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.EfCore.Abstractions;
using CQ.ServiceExtension;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CQ.UnitOfWork.EfCore
{
    public static class EfCoreDependencyInjection
    {
        /// <summary>
        /// Base on config, injects TContext under TContext, EfCoreContext, IDatabaseContext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="lifeTime"></param>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddEfCoreContext<TContext>(
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

            services
                .AddDbContext<TContext>(actions, lifeTimeDb)
                .AddDbContext<EfCoreContext, TContext>(actions, (ServiceLifetime)lifeTime)
                .AddService<IDatabaseContext, TContext>(lifeTime);

            return services;
        }

        /// <summary>
        /// Injects EfCoreRepository under IRepository, IUnitRepository and IEfCoreRepository
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddEfCoreRepository<TEntity>(this IServiceCollection services, LifeTime lifeTime = LifeTime.Scoped) where TEntity : class
        {
            services
                .AddService<IRepository<TEntity>, EfCoreRepository<TEntity>>(lifeTime)
                .AddService<IUnitRepository<TEntity>, EfCoreRepository<TEntity>>(lifeTime)
                .AddService<IEfCoreRepository<TEntity>, EfCoreRepository<TEntity>>(lifeTime);

            return services;
        }

        /// <summary>
        /// Injects TRepository under IRepository, IUnitRepository and IEfCoreRepository
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddCustomEfCoreRepository<TEntity, TRepository>(this IServiceCollection services, LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : EfCoreRepository<TEntity>
        {
            services
                .AddService<IRepository<TEntity>, TRepository>(lifeTime)
                .AddService<IUnitRepository<TEntity>, TRepository>(lifeTime)
                .AddService<IEfCoreRepository<TEntity>, TRepository>(lifeTime);

            return services;
        }

        /// <summary>
        /// Injects EfCoreRepository using TContext under IRepository, IUnitRepository and IEfCoreRepository
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddEfCoreRepository<TEntity, TContext>(this IServiceCollection services, LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TContext : EfCoreContext
        {
            var repositoryImplementation = (IServiceProvider serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<TContext>();
                return new EfCoreRepository<TEntity>(context);
            };

            services
                .AddService<IRepository<TEntity>>(repositoryImplementation, lifeTime)
                .AddService<IUnitRepository<TEntity>>(repositoryImplementation, lifeTime)
                .AddService<IEfCoreRepository<TEntity>>(repositoryImplementation, lifeTime);

            return services;
        }

        /// <summary>
        /// Injects TImplementation under IRepository, IUnitRepository, IEfCoreRepository and TRepository
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddCustomEfCoreRepository<TEntity, TRepository, TImplementation>(this IServiceCollection services, LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : class
            where TImplementation : EfCoreRepository<TEntity>, TRepository
        {
            services
                .AddService<IRepository<TEntity>, TImplementation>(lifeTime)
                .AddService<IUnitRepository<TEntity>, TImplementation>(lifeTime)
                .AddService<IEfCoreRepository<TEntity>, TImplementation>(lifeTime)
                .AddService<TRepository, TImplementation>(lifeTime);

            return services;
        }
    }
}