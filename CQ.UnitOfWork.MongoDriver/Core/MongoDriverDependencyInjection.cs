using CQ.UnitOfWork.Abstractions;
using CQ.ServiceExtension;
using CQ.UnitOfWork.MongoDriver.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;

namespace CQ.UnitOfWork.MongoDriver
{
    public static class MongoDriverDependencyInjection
    {
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

            var contextImplementation = (IServiceProvider serviceProvider) =>
            {
                var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
                var mongoDatabase = mongoClient.GetDatabase(config.DatabaseConnection.DatabaseName);
                return new MongoContext(mongoDatabase);
            };

            services
                .AddService<IMongoClient>((serviceProvider) =>
                {
                    var mongoClientSettings = MongoClientSettings.FromConnectionString(config.DatabaseConnection.ConnectionString);
                    mongoClientSettings.ClusterConfigurator = BuildClusterConfigurator(config.ClusterConfigurator, config.UseDefaultQueryLogger);
                    var mongoClient = new MongoClient(mongoClientSettings);
                    return mongoClient;
                }, mongoClientLifeTime)
                .AddService(contextImplementation, contextLifeTime)
                .AddService<IDatabaseContext>(contextImplementation, contextLifeTime);

            return services;
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

        /// Injects MongoDriverRepository under IRepository, IUnitRepository, IMongoDriverRepository, and use defaultMongoContext
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="collectionName"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddMongoRepository<TEntity>(this IServiceCollection services, string? collectionName = null, LifeTime lifeTime = LifeTime.Scoped) where TEntity : class
        {
            var implementationFactory = (IServiceProvider serviceProvider) =>
            {
                var mongoContext = serviceProvider.GetRequiredService<MongoContext>();

                return new MongoDriverRepository<TEntity>(mongoContext, collectionName);
            };

            services
                .AddService<IRepository<TEntity>>(implementationFactory, lifeTime)
                .AddService<IUnitRepository<TEntity>>(implementationFactory, lifeTime)
                .AddService<IMongoDriverRepository<TEntity>>(implementationFactory, lifeTime);

            return services;
        }

        /// <summary>
        /// Injects TRepository under IRepository, IUnitRepository, IMongoDriverRepository, and use defaultMongoContext
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddMongoRepository<TEntity, TRepository>(this IServiceCollection services, LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : MongoDriverRepository<TEntity>
        {

            services
                .AddService<IRepository<TEntity>, TRepository>(lifeTime)
                .AddService<IUnitRepository<TEntity>, TRepository>(lifeTime)
                .AddService<IMongoDriverRepository<TEntity>, TRepository>(lifeTime);

            return services;
        }

        /// <summary>
        /// Injects TImplementation under IRepository, IUnitRepository, IMongoDriverRepository, TRepository, and use defaultMongoContext
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static IServiceCollection AddCustomMongoRepository<TEntity, TRepository, TImplementation>(this IServiceCollection services, LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : class
            where TImplementation : MongoDriverRepository<TEntity>, TRepository
        {
            services
                .AddService<IRepository<TEntity>, TImplementation>(lifeTime)
                .AddService<IUnitRepository<TEntity>, TImplementation>(lifeTime)
                .AddService<IMongoDriverRepository<TEntity>, TImplementation>(lifeTime)
                .AddService<TRepository, TImplementation>(lifeTime);

            return services;
        }
    }
}
