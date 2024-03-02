using CQ.UnitOfWork.Abstractions;
using CQ.ServiceExtension;
using CQ.UnitOfWork.MongoDriver.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;
using System.Collections;
using CQ.Utility;
using System;

namespace CQ.UnitOfWork.MongoDriver
{
    public static class MongoDriverDependencyInjection
    {
        #region Add Context
        /// <summary>
        /// Base on the config, injects IMongoClient and MongoContext
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="contextLifeTime"></param>
        /// <param name="mongoClientLifeTime"></param>
        public static IServiceCollection AddMongoContext(
            this IServiceCollection services,
            MongoConfig config,
            LifeTime contextLifeTime = LifeTime.Scoped,
            LifeTime mongoClientLifeTime = LifeTime.Scoped,
            LifeTime configLifeTime = LifeTime.Scoped)
        {
            Guard.ThrowIsNull(config, nameof(config));

            var clientSettings = MongoClientSettings.FromConnectionString(config.DatabaseConnection.ConnectionString);
            clientSettings.ClusterConfigurator = BuildClusterConfigurator(config.ClusterConfigurator, config.UseDefaultQueryLogger);

            var contextImplementation = (IServiceProvider serviceProvider) =>
            {
                var mongoDatabase = BuildMongoDatabase(config, serviceProvider, clientSettings);

                return new MongoContext(mongoDatabase);
            };

            services
                .AddService<IMongoClient>((serviceProvider) => BuildMongoClient(clientSettings), mongoClientLifeTime)
                .AddService<MongoConfig>((provider) => config, configLifeTime)
                .AddService<MongoContext>(contextImplementation, contextLifeTime)
                .AddService<IDatabaseContext, MongoContext>(contextImplementation, contextLifeTime);

            return services;
        }

        /// <summary>
        /// Base on the config, injects IMongoClient, MongoContext and TContext
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="contextLifeTime"></param>
        /// <param name="mongoClientLifeTime"></param>
        public static IServiceCollection AddMongoContext<TContext>(
            this IServiceCollection services,
            MongoConfig config,
            LifeTime contextLifeTime = LifeTime.Scoped,
            LifeTime mongoClientLifeTime = LifeTime.Scoped,
            LifeTime configLifeTime = LifeTime.Scoped)
            where TContext : MongoContext
        {
            Guard.ThrowIsNull(config, nameof(config));

            var clientSettings = MongoClientSettings.FromConnectionString(config.DatabaseConnection.ConnectionString);
            clientSettings.ClusterConfigurator = BuildClusterConfigurator(config.ClusterConfigurator, config.UseDefaultQueryLogger);

            var contextImplementation = (IServiceProvider serviceProvider) =>
            {
                var mongoDatabase = BuildMongoDatabase(config, serviceProvider, clientSettings);

                return (TContext)Activator.CreateInstance(typeof(TContext), mongoDatabase)!;
            };

            services
                .AddService<IMongoClient>((serviceProvider) => BuildMongoClient(clientSettings), mongoClientLifeTime)
                .AddService<MongoConfig>((provider) => config, configLifeTime)
                .AddService<TContext>(contextImplementation, contextLifeTime)
                .AddService<MongoContext, TContext>(contextImplementation, contextLifeTime)
                .AddService<IDatabaseContext, TContext>(contextImplementation, contextLifeTime);

            return services;
        }

        private static MongoClient BuildMongoClient(MongoClientSettings clientSettings)
        {
            var mongoClient = new MongoClient(clientSettings);

            return mongoClient;
        }

        private static IMongoDatabase BuildMongoDatabase(
                MongoConfig config,
                IServiceProvider serviceProvider,
                MongoClientSettings clientSettings)
        {
            var mongoClients = serviceProvider.GetRequiredService<IEnumerable<IMongoClient>>();

            var mongoClient = mongoClients.First(m => m.Settings.Server.Host == clientSettings.Server.Host && m.Settings.Server.Port == clientSettings.Server.Port);

            var mongoDatabase = mongoClient.GetDatabase(config.DatabaseConnection.Name);

            return mongoDatabase;
        }

        private static Action<ClusterBuilder>? BuildClusterConfigurator(Action<ClusterBuilder>? clusterConfigurator = null, bool useDefaultClusterConfigurator = false)
        {
            Action<ClusterBuilder>? defaultClusterConfigurator = useDefaultClusterConfigurator ? cb =>
            {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                    Console.WriteLine($"{e.CommandName} - {e.Command.ToJson(new JsonWriterSettings { Indent = true })}");
                    Console.WriteLine(new String('-', 32));
                });
            }
            : null;

            clusterConfigurator ??= defaultClusterConfigurator;

            return clusterConfigurator;
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
                var defaultContext = BuildDefaultContext(provider);

                return new MongoDriverRepository<TEntity>(defaultContext);
            };

            services.AddRepository<TEntity>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddRepository<TEntity, TContext>(
            this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TContext : MongoContext
        {
            var repositoryFactory = (IServiceProvider provider) =>
            {
                var specificContext = BuildContext<TContext>(provider);

                return new MongoDriverRepository<TEntity>(specificContext);
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
            Guard.ThrowIsNullOrEmpty(databaseName, nameof(databaseName));

            var repositoryFactory = (IServiceProvider provider) =>
            {
                var defaultContext = BuildContext(provider, databaseName);

                return new MongoDriverRepository<TEntity>(defaultContext);
            };

            services.AddRepository<TEntity>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddMongoRepository<TEntity>(
            this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
        {
            var repositoryFactory = (IServiceProvider provider) =>
            {
                var defaultContext = BuildDefaultContext(provider);

                return new MongoDriverRepository<TEntity>(defaultContext);
            };

            services.AddRepository<TEntity>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddMongoRepository<TEntity, TContext>(
            this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TContext : MongoContext
        {
            var repositoryFactory = (IServiceProvider provider) =>
            {
                var specificContext = BuildContext<TContext>(provider);

                return new MongoDriverRepository<TEntity>(specificContext);
            };

            services.AddRepository<TEntity>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddMongoRepository<TEntity>(
            this IServiceCollection services,
            string databaseName,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
        {
            Guard.ThrowIsNullOrEmpty(databaseName, nameof(databaseName));

            var repositoryFactory = (IServiceProvider provider) =>
            {
                var defaultContext = BuildContext(provider, databaseName);

                return new MongoDriverRepository<TEntity>(defaultContext);
            };

            services.AddRepository<TEntity>(repositoryFactory, lifeTime);

            return services;
        }

        private static IServiceCollection AddRepository<TEntity>(
            this IServiceCollection services,
            Func<IServiceProvider, MongoDriverRepository<TEntity>> repositoryFactory,
            LifeTime lifeTime)
            where TEntity : class
        {
            services
                .AddService<IRepository<TEntity>, MongoDriverRepository<TEntity>>(repositoryFactory, lifeTime)
                .AddService<IUnitRepository<TEntity>, MongoDriverRepository<TEntity>>(repositoryFactory, lifeTime)
                .AddService<IMongoDriverRepository<TEntity>, MongoDriverRepository<TEntity>>(repositoryFactory, lifeTime);

            return services;
        }
        #endregion

        #region AddCustomMongoRepository
        /// <summary>
        /// Injects TRepository under IRepository, IUnitRepository, IMongoDriverRepository, and use defaultMongoContext
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TCustomRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddCustomMongoRepository<TEntity, TCustomRepository>(
            this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TCustomRepository : MongoDriverRepository<TEntity>
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                var context = BuildDefaultContext(services);

                return (TCustomRepository)Activator.CreateInstance(typeof(TCustomRepository), context)!;
            };

            services.AddCustomMongoRepository<TEntity, TCustomRepository>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddCustomMongoRepository<TEntity, TCustomRepository, TContext>(
            this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TCustomRepository : MongoDriverRepository<TEntity>
            where TContext : MongoContext
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                var context = BuildContext<TContext>(services);

                return (TCustomRepository)Activator.CreateInstance(typeof(TCustomRepository), context)!;
            };

            services.AddCustomMongoRepository<TEntity, TCustomRepository>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddCustomMongoRepository<TEntity, TCustomRepository>(
            this IServiceCollection services,
            string databaseName,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TCustomRepository : MongoDriverRepository<TEntity>
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                var context = BuildContext(services, databaseName);

                return (TCustomRepository)Activator.CreateInstance(typeof(TCustomRepository), context)!;
            };

            services.AddCustomMongoRepository<TEntity, TCustomRepository>(repositoryFactory, lifeTime);

            return services;
        }

        private static IServiceCollection AddCustomMongoRepository<TEntity, TCustomRepository>(
            this IServiceCollection services,
            Func<IServiceProvider, TCustomRepository> repositoryFactory,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TCustomRepository : MongoDriverRepository<TEntity>
        {
            services
                .AddService<IRepository<TEntity>, TCustomRepository>(repositoryFactory, lifeTime)
                .AddService<IUnitRepository<TEntity>, TCustomRepository>(repositoryFactory, lifeTime)
                .AddService<IMongoDriverRepository<TEntity>, TCustomRepository>(repositoryFactory, lifeTime);

            return services;
        }

        #endregion

        #region AddAbstractionMongoRepository
        /// <summary>
        /// Injects TImplementation under IRepository, IUnitRepository, IMongoDriverRepository, TRepository, and use defaultMongoContext
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddAbstractionMongoRepository<TEntity, TRepository, TImplementation>
            (this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : class
            where TImplementation : MongoDriverRepository<TEntity>, TRepository
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                var context = BuildDefaultContext(services);

                return (TImplementation)Activator.CreateInstance(typeof(TImplementation), context)!;
            };

            services.AddAbstractionMongoRepository<TEntity, TRepository, TImplementation>(repositoryFactory, lifeTime);

            return services;
        }

        public static IServiceCollection AddAbstractionMongoRepository<TEntity, TRepository, TImplementation, TContext>
            (this IServiceCollection services,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : class
            where TImplementation : MongoDriverRepository<TEntity>, TRepository
            where TContext : MongoContext
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                var context = BuildContext<TContext>(services);

                return (TImplementation)Activator.CreateInstance(typeof(TImplementation), context)!;
            };

            services.AddAbstractionMongoRepository<TEntity, TRepository, TImplementation>(repositoryFactory, lifeTime);

            return services;
        }
        
        public static IServiceCollection AddAbstractionMongoRepository<TEntity, TRepository, TImplementation>
            (this IServiceCollection services,
            string databaseName,
            LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : class
            where TImplementation : MongoDriverRepository<TEntity>, TRepository
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                var context = BuildContext(services, databaseName);

                return (TImplementation)Activator.CreateInstance(typeof(TImplementation), context)!;
            };

            services.AddAbstractionMongoRepository<TEntity, TRepository, TImplementation>(repositoryFactory, lifeTime);

            return services;
        }

        private static IServiceCollection AddAbstractionMongoRepository<TEntity, TRepository, TImplementation>(
            this IServiceCollection services,
            Func<IServiceProvider, TImplementation> repositoryFactory,
            LifeTime lifeTime)
            where TEntity : class
            where TRepository : class
            where TImplementation : MongoDriverRepository<TEntity>, TRepository
        {
            services
                .AddService<IRepository<TEntity>, TImplementation>(repositoryFactory, lifeTime)
                .AddService<IUnitRepository<TEntity>, TImplementation>(repositoryFactory, lifeTime)
                .AddService<IMongoDriverRepository<TEntity>, TImplementation>(repositoryFactory, lifeTime)
                .AddService<TRepository, TImplementation>(repositoryFactory, lifeTime);

            return services;
        }
        #endregion

        private static MongoContext BuildDefaultContext(IServiceProvider provider)
        {
            var configs = provider.GetRequiredService<IEnumerable<MongoConfig>>();
            var defaultConfig = configs.FirstOrDefault(c => c.Default);

            Guard.ThrowIsNull(defaultConfig, "default config");

            var contexts = provider.GetRequiredService<IEnumerable<MongoContext>>();
            var defaultContext = contexts.FirstOrDefault(c => c.GetDatabaseInfo().Name == defaultConfig.DatabaseConnection.Name);

            Guard.ThrowIsNull(defaultConfig, "mongo context");

            return defaultContext;
        }

        private static MongoContext BuildContext<TContext>(IServiceProvider provider)
            where TContext : MongoContext
        {
            var specificContext = provider.GetRequiredService<TContext>();

            if (specificContext == null)
            {
                throw new Exception($"Missing context: {typeof(TContext).Name}");
            }

            return specificContext;
        }

        private static MongoContext BuildContext(IServiceProvider provider, string databaseName)
        {
            var contexts = provider.GetRequiredService<IEnumerable<MongoContext>>();
            var specificContext = contexts.FirstOrDefault(c => c.GetDatabaseInfo().Name == databaseName);

            Guard.ThrowIsNull(specificContext, $"context of {databaseName}");

            return specificContext;
        }
    }
}
