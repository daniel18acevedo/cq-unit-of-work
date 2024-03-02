using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.EfCore.Abstractions;
using CQ.ServiceExtension;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CQ.Utility;
using Microsoft.Extensions.Options;

namespace CQ.UnitOfWork.EfCore
{
    public static class EfCoreDependencyInjection
    {
        #region Add Context
        /// <summary>
        /// Base on config, injects TContext under TContext, EfCoreContext, IDatabaseContext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="contextLifeTime"></param>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddEfCoreContext<TContext>(
            this IServiceCollection services,
            EfCoreConfig config,
            LifeTime contextLifeTime = LifeTime.Scoped,
            LifeTime configLifeTime = LifeTime.Scoped)
            where TContext : EfCoreContext
        {
            Guard.ThrowIsNull(config, nameof(config));

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
                else if (config.Logger != null)
                {
                    optionsBuilder.LogTo(config.Logger);
                }
            };

            var lifeTimeDb = contextLifeTime == LifeTime.Scoped
                ? ServiceLifetime.Scoped
                : contextLifeTime == LifeTime.Transient
                ? ServiceLifetime.Transient
                : ServiceLifetime.Singleton;

            services
                .AddService<EfCoreConfig>((provider) => config, configLifeTime)
                .AddDbContext<TContext>(actions, lifeTimeDb)
                .AddService<EfCoreContext, TContext>((provider) =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<TContext>()
                    .UseSqlServer(config.DatabaseConnection.ConnectionString);

                    if (config.UseDefaultQueryLogger)
                    {
                        optionsBuilder.LogTo(Console.WriteLine);
                    }
                    else if (config.Logger != null)
                    {
                        optionsBuilder.LogTo(config.Logger);
                    }

                    var options = optionsBuilder.Options;

                    return (TContext)Activator.CreateInstance(typeof(TContext), options);
                }, contextLifeTime)
                .AddService<IDatabaseContext, TContext>(contextLifeTime);

            return services;
        }
        #endregion

        #region AddRepository
        public static IServiceCollection AddRepository<TEntity>(
            this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
        {
            var repositoryFactory = (IServiceProvider provider) =>
            {
                var context = BuildDefaultContext(provider);

                return new EfCoreRepository<TEntity>(context);
            };

            services.AddRepository<TEntity>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddRepository<TEntity, TContext>(
            this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TContext : EfCoreContext
        {
            var repositoryFactory = (IServiceProvider provider) =>
            {
                var context = BuildContext<TContext>(provider);

                return new EfCoreRepository<TEntity>(context);
            };

            services.AddRepository<TEntity>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddRepository<TEntity>(
            this IServiceCollection services,
            string databaseName,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
        {
            var repositoryFactory = (IServiceProvider provider) =>
            {
                var context = BuildContext(provider, databaseName);

                return new EfCoreRepository<TEntity>(context);
            };

            services.AddRepository<TEntity>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddEfCoreRepository<TEntity>(
            this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
        {
            var repositoryFactory = (IServiceProvider provider) =>
            {
                var context = BuildDefaultContext(provider);

                return new EfCoreRepository<TEntity>(context);
            };

            services.AddRepository<TEntity>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddEfCoreRepository<TEntity, TContext>(
            this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TContext : EfCoreContext
        {
            var repositoryFactory = (IServiceProvider provider) =>
            {
                var context = BuildContext<TContext>(provider);

                return new EfCoreRepository<TEntity>(context);
            };

            services.AddRepository<TEntity>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddEfCoreRepository<TEntity>(
            this IServiceCollection services,
            string databaseName,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
        {
            var repositoryFactory = (IServiceProvider provider) =>
            {
                var context = BuildContext(provider, databaseName);

                return new EfCoreRepository<TEntity>(context);
            };

            services.AddRepository<TEntity>(repositoryFactory, lifeTime);

            return services;
        }

        private static IServiceCollection AddRepository<TEntity>(
            this IServiceCollection services,
            Func<IServiceProvider, EfCoreRepository<TEntity>> repositoryFactory,
            LifeTime lifeTime)
            where TEntity : class
        {
            services
                .AddService<IRepository<TEntity>>(repositoryFactory, lifeTime)
                .AddService<IUnitRepository<TEntity>>(repositoryFactory, lifeTime)
                .AddService<IEfCoreRepository<TEntity>>(repositoryFactory, lifeTime);

            return services;
        }
        #endregion

        #region AddCustomEfCoreRepository
        public static IServiceCollection AddCustomEfCoreRepository<TEntity, TCustomRepository>(
            this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TCustomRepository : EfCoreRepository<TEntity>
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                var context = BuildDefaultContext(services);

                return (TCustomRepository)Activator.CreateInstance(typeof(TCustomRepository), context)!;
            };

            services.AddCustomEfCoreRepository<TEntity, TCustomRepository>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddCustomEfCoreRepository<TEntity, TCustomRepository, TContext>(
            this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TCustomRepository : EfCoreRepository<TEntity>
            where TContext : EfCoreContext
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                var context = BuildContext<TContext>(services);

                return (TCustomRepository)Activator.CreateInstance(typeof(TCustomRepository), context)!;
            };

            services.AddCustomEfCoreRepository<TEntity, TCustomRepository>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddCustomEfCoreRepository<TEntity, TCustomRepository>(
            this IServiceCollection services,
            string databaseName,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TCustomRepository : EfCoreRepository<TEntity>
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                var context = BuildContext(services, databaseName);

                return (TCustomRepository)Activator.CreateInstance(typeof(TCustomRepository), context)!;
            };

            services.AddCustomEfCoreRepository<TEntity, TCustomRepository>(repositoryFactory, lifeTime);

            return services;
        }

        private static IServiceCollection AddCustomEfCoreRepository<TEntity, TCustomRepository>(
            this IServiceCollection services,
            Func<IServiceProvider, TCustomRepository> repositoryFactory,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TCustomRepository : EfCoreRepository<TEntity>
        {
            services
                .AddService<IRepository<TEntity>, TCustomRepository>(repositoryFactory, lifeTime)
                .AddService<IUnitRepository<TEntity>, TCustomRepository>(repositoryFactory, lifeTime)
                .AddService<IEfCoreRepository<TEntity>, TCustomRepository>(repositoryFactory, lifeTime);

            return services;
        }
        #endregion

        #region AddAbstractionEfCoreRepository
        /// <summary>
        /// Injects TImplementation under IRepository, IUnitRepository, IMongoDriverRepository, TRepository, and use defaultMongoContext
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddAbstractionEfCoreRepository<TEntity, TRepository, TImplementation>
            (this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : class
            where TImplementation : EfCoreRepository<TEntity>, TRepository
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                var context = BuildDefaultContext(services);

                return (TImplementation)Activator.CreateInstance(typeof(TImplementation), context)!;
            };

            services.AddAbstractionEfCoreRepository<TEntity, TRepository, TImplementation>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddAbstractionEfCoreRepository<TEntity, TRepository, TImplementation, TContext>
            (this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : class
            where TImplementation : EfCoreRepository<TEntity>, TRepository
            where TContext : EfCoreContext
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                var context = BuildContext<TContext>(services);

                return (TImplementation)Activator.CreateInstance(typeof(TImplementation), context)!;
            };

            services.AddAbstractionEfCoreRepository<TEntity, TRepository, TImplementation>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddAbstractionEfCoreRepository<TEntity, TRepository, TImplementation>
            (this IServiceCollection services,
            string databaseName,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : class
            where TImplementation : EfCoreRepository<TEntity>, TRepository
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                var context = BuildContext(services, databaseName);

                return (TImplementation)Activator.CreateInstance(typeof(TImplementation), context)!;
            };

            services.AddAbstractionEfCoreRepository<TEntity, TRepository, TImplementation>(repositoryFactory, lifeTime);

            return services;
        }

        private static IServiceCollection AddAbstractionEfCoreRepository<TEntity, TRepository, TImplementation>(
            this IServiceCollection services,
            Func<IServiceProvider, TImplementation> repositoryFactory,
            LifeTime lifeTime)
            where TEntity : class
            where TRepository : class
            where TImplementation : EfCoreRepository<TEntity>, TRepository
        {
            services
                .AddService<IRepository<TEntity>, TImplementation>(repositoryFactory, lifeTime)
                .AddService<IUnitRepository<TEntity>, TImplementation>(repositoryFactory, lifeTime)
                .AddService<IEfCoreRepository<TEntity>, TImplementation>(repositoryFactory, lifeTime)
                .AddService<TRepository, TImplementation>(repositoryFactory, lifeTime);

            return services;
        }
        #endregion

        private static EfCoreContext BuildDefaultContext(IServiceProvider provider)
        {
            var configs = provider.GetRequiredService<IEnumerable<EfCoreConfig>>();
            var defaultConfig = configs.FirstOrDefault(c => c.Default);

            Guard.ThrowIsNull(defaultConfig, "default config");

            var contexts = provider.GetRequiredService<IEnumerable<EfCoreContext>>();
            var defaultContext = contexts.FirstOrDefault(c => c.GetDatabaseInfo().Name == defaultConfig.DatabaseConnection.Name);

            return defaultContext;
        }

        private static EfCoreContext BuildContext<TContext>(IServiceProvider provider)
            where TContext : EfCoreContext
        {
            var specificContext = provider.GetRequiredService<TContext>();

            Guard.ThrowIsNull(specificContext, $"context {typeof(TContext).Name}");

            return specificContext;
        }

        private static EfCoreContext BuildContext(IServiceProvider provider, string databaseName)
        {
            var contexts = provider
                .GetRequiredService<IEnumerable<EfCoreContext>>();

            var databaseInfo = contexts.Select(c => c.GetDatabaseInfo());

            var context = contexts.FirstOrDefault(c => c.GetDatabaseInfo().Name == databaseName);

            Guard.ThrowIsNull(context, $"context {databaseName}");

            return context;
        }
    }
}