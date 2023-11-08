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
            LifeTime mongoClientLifeTime = LifeTime.Singleton)
        {
            config.Assert();

            var clientSettings = MongoClientSettings.FromConnectionString(config.DatabaseConnection.ConnectionString);

            var contextImplementation = (IServiceProvider serviceProvider) =>
            {
                IMongoDatabase mongoDatabase = BuildMongoDatabase(config, serviceProvider, clientSettings);

                return new MongoContext(mongoDatabase, config.DefaultToUse);
            };

            services
                .AddService<IMongoClient>((serviceProvider) =>
                {
                    clientSettings.ClusterConfigurator = BuildClusterConfigurator(config.ClusterConfigurator, config.UseDefaultQueryLogger);
                    var mongoClient = new MongoClient(clientSettings);
                    return mongoClient;
                }, mongoClientLifeTime)
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
            LifeTime mongoClientLifeTime = LifeTime.Singleton)
            where TContext : MongoContext
        {
            config.Assert();

            var clientSettings = MongoClientSettings.FromConnectionString(config.DatabaseConnection.ConnectionString);

            var contextImplementation = (IServiceProvider serviceProvider) =>
            {
                IMongoDatabase mongoDatabase = BuildMongoDatabase(config, serviceProvider, clientSettings);

                return (TContext)Activator.CreateInstance(typeof(TContext), mongoDatabase, config.DefaultToUse);
            };

            services
                .AddService<IMongoClient>((serviceProvider) =>
                {
                    clientSettings.ClusterConfigurator = BuildClusterConfigurator(config.ClusterConfigurator, config.UseDefaultQueryLogger);
                    var mongoClient = new MongoClient(clientSettings);
                    return mongoClient;
                }, mongoClientLifeTime)
                .AddService<TContext>(contextImplementation, contextLifeTime)
                .AddService<MongoContext, TContext>(contextImplementation, contextLifeTime)
                .AddService<IDatabaseContext, TContext>(contextImplementation, contextLifeTime);

            return services;
        }

        private static IMongoDatabase BuildMongoDatabase(MongoConfig config, IServiceProvider serviceProvider, MongoClientSettings clientSettings)
        {
            var mongoClients = serviceProvider.GetRequiredService<IEnumerable<IMongoClient>>();

            var mongoClient = mongoClients.FirstOrDefault(m => m.Settings.Server.Host == clientSettings.Server.Host && m.Settings.Server.Port == clientSettings.Server.Port);

            var mongoDatabase = mongoClient.GetDatabase(config.DatabaseConnection.DatabaseName);
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

        /// Injects MongoDriverRepository under IRepository, IUnitRepository, IMongoDriverRepository, and use defaultMongoContext
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="collectionName"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddMongoRepository<TEntity>(this IServiceCollection services, string? databaseName = null, LifeTime lifeTime = LifeTime.Scoped) where TEntity : class
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                MongoContext context = BuildMongoContext(databaseName, services);

                return new MongoDriverRepository<TEntity>(context);
            };

            services
                .AddService<IRepository<TEntity>, MongoDriverRepository<TEntity>>(repositoryFactory, lifeTime)
                .AddService<IUnitRepository<TEntity>, MongoDriverRepository<TEntity>>(repositoryFactory, lifeTime)
                .AddService<IMongoDriverRepository<TEntity>, MongoDriverRepository<TEntity>>(repositoryFactory, lifeTime);

            return services;
        }

        private static MongoContext BuildMongoContext(string? databaseName, IServiceProvider services)
        {
            var contexts = services.GetRequiredService<IEnumerable<MongoContext>>();

            MongoContext context;
            if (databaseName != null)
            {
                context = contexts.FirstOrDefault(c => c.GetDatabaseInfo().Name == databaseName);
            }
            else
            {
                context = contexts.FirstOrDefault(c => c.IsDefault);
            }

            context ??= contexts.First();

            return context;
        }

        public static IServiceCollection AddMongoRepositoryContext<TEntity, TContext>(this IServiceCollection services, LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TContext : MongoContext
        {
            var implementationFactory = (IServiceProvider services) =>
            {
                var mongoContext = services.GetRequiredService<TContext>();

                return new MongoDriverRepository<TEntity>(mongoContext);
            };

            services
                .AddService<IRepository<TEntity>, MongoDriverRepository<TEntity>>(implementationFactory, lifeTime)
                .AddService<IUnitRepository<TEntity>, MongoDriverRepository<TEntity>>(implementationFactory, lifeTime)
                .AddService<IMongoDriverRepository<TEntity>, MongoDriverRepository<TEntity>>(implementationFactory, lifeTime);

            return services;
        }

        /// <summary>
        /// Injects TRepository under IRepository, IUnitRepository, IMongoDriverRepository, and use defaultMongoContext
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddMongoRepository<TEntity, TRepository>(this IServiceCollection services, string? databaseName = null, LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : MongoDriverRepository<TEntity>
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                MongoContext context = BuildMongoContext(databaseName, services);

                return (TRepository)Activator.CreateInstance(typeof(TRepository), context);
            };

            services
                .AddService<IRepository<TEntity>, TRepository>(repositoryFactory, lifeTime)
                .AddService<IUnitRepository<TEntity>, TRepository>(repositoryFactory, lifeTime)
                .AddService<IMongoDriverRepository<TEntity>, TRepository>(repositoryFactory, lifeTime);

            return services;
        }

        /// <summary>
        /// Injects TImplementation under IRepository, IUnitRepository, IMongoDriverRepository, TRepository, and use defaultMongoContext
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddCustomMongoRepository<TEntity, TRepository, TImplementation>(this IServiceCollection services, string? databaseName = null, LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : class
            where TImplementation : MongoDriverRepository<TEntity>, TRepository
        {
            var repositoryFactory = (IServiceProvider services) =>
            {
                MongoContext context = BuildMongoContext(databaseName, services);

                return (TImplementation)Activator.CreateInstance(typeof(TImplementation), context);
            };

            services
                .AddService<IRepository<TEntity>, TImplementation>(repositoryFactory, lifeTime)
                .AddService<IUnitRepository<TEntity>, TImplementation>(repositoryFactory, lifeTime)
                .AddService<IMongoDriverRepository<TEntity>, TImplementation>(repositoryFactory, lifeTime)
                .AddService<TRepository, TImplementation>(repositoryFactory, lifeTime);

            return services;
        }
    }
}
