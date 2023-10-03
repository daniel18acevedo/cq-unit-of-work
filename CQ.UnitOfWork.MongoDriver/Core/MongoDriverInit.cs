using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Extensions;
using CQ.UnitOfWork.MongoDriver.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;

namespace CQ.UnitOfWork.MongoDriver
{
    public static class MongoDriverInit
    {
        public static void AddMongoContext(
            this IServiceCollection services,
            MongoConfig config,
            LifeTime contextLifeTime = LifeTime.Scoped,
            LifeTime mongoClientLifeTime = LifeTime.Singleton)
        {
            config.Assert();

            services.AddService<IMongoClient>((serviceProvider) =>
            {
                var mongoClientSettings = MongoClientSettings.FromConnectionString(config.DatabaseConnection.ConnectionString);
                mongoClientSettings.ClusterConfigurator = BuildClusterConfigurator(config.ClusterConfigurator, config.UseDefaultQueryLogger);

                var mongoClient = new MongoClient(mongoClientSettings);

                return mongoClient;
            }, mongoClientLifeTime);

            var contextImplementation = (IServiceProvider serviceProvider) =>
            {
                var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();

                var mongoDatabase = mongoClient.GetDatabase(config.DatabaseConnection.DatabaseName);

                return new MongoContext(mongoDatabase);
            };

            services.AddService(contextImplementation, contextLifeTime);
            services.AddService<IDatabaseContext>(contextImplementation, contextLifeTime);
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
        
        /// <summary>
        /// Use default mongo context
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="collectionName"></param>
        /// <param name="lifeTime"></param>
        public static void AddMongoRepository<TEntity>(this IServiceCollection services, string? collectionName = null, LifeTime lifeTime = LifeTime.Scoped) where TEntity : class
        {
            var implementationFactory = (IServiceProvider serviceProvider) =>
            {
                var mongoContext = serviceProvider.GetRequiredService<MongoContext>();

                return new MongoDriverRepository<TEntity>(mongoContext, collectionName);
            };

            services.AddService<IRepository<TEntity>>(implementationFactory, lifeTime);
            services.AddService<IUnitRepository<TEntity>>(implementationFactory, lifeTime);
            services.AddService<IMongoDriverRepository<TEntity>>(implementationFactory, lifeTime);
        }

        /// <summary>
        /// Use specific context. Usefull when are multiple ef core context defined.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static void AddMongoRepository<TEntity, TRepository>(this IServiceCollection services, LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : MongoDriverRepository<TEntity>
        {

            services.AddService<IRepository<TEntity>, TRepository>(lifeTime);
            services.AddService<IUnitRepository<TEntity>, TRepository>(lifeTime);
            services.AddService<IMongoDriverRepository<TEntity>, TRepository>(lifeTime);
        }

        /// <summary>
        /// Sets custom repository under repository interfaces.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifeTime"></param>
        public static void AddCustomMongoRepository<TEntity, TRepository>(this IServiceCollection services, LifeTime lifeTime = LifeTime.Scoped)
            where TEntity : class
            where TRepository : MongoDriverRepository<TEntity>
        {

            services.AddService<IRepository<TEntity>, TRepository>(lifeTime);
            services.AddService<IUnitRepository<TEntity>, TRepository>(lifeTime);
            services.AddService<IMongoDriverRepository<TEntity>, TRepository>(lifeTime);
        }
    }
}
