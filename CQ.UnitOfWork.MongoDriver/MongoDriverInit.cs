using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Abstractions.Extensions;
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
            LifeCycle contextLifeCycle = LifeCycle.SCOPED,
            LifeCycle mongoClientLifeCycle = LifeCycle.SINGLETON)
        {
            config.Assert();

            services.AddService<IMongoClient>((serviceProvider) =>
            {
                var mongoClientSettings = MongoClientSettings.FromConnectionString(config.DatabaseConnection.ConnectionString);
                mongoClientSettings.ClusterConfigurator = BuildClusterConfigurator(config.ClusterConfigurator, config.UseDefaultQueryLogger);

                var mongoClient = new MongoClient(mongoClientSettings);

                return mongoClient;
            }, mongoClientLifeCycle);

            var contextImplementation = (IServiceProvider serviceProvider) =>
            {
                var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();

                var mongoDatabase = mongoClient.GetDatabase(config.DatabaseConnection.DatabaseName);

                return new MongoContext(mongoDatabase);
            };

            services.AddService(contextImplementation, contextLifeCycle);
            services.AddService<IDatabaseContext>(contextImplementation, contextLifeCycle);
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

        public static void AddMongoRepository<TEntity>(this IServiceCollection services, string? collectionName = null, LifeCycle lifeCycle = LifeCycle.SCOPED) where TEntity : class
        {
            var implementationFactory = (IServiceProvider serviceProvider) =>
            {
                var mongoContext = serviceProvider.GetRequiredService<MongoContext>();

                return new MongoDriverRepository<TEntity>(mongoContext, collectionName);
            };

            services.AddService<IRepository<TEntity>>(implementationFactory, lifeCycle);
            services.AddService<IMongoDriverRepository<TEntity>>(implementationFactory, lifeCycle);
        }

        public static void AddMongoRepository<TEntity, TRepository>(this IServiceCollection services, LifeCycle lifeCycle = LifeCycle.SCOPED)
            where TEntity : class
            where TRepository : MongoDriverRepository<TEntity>
        {

            services.AddService<IRepository<TEntity>, TRepository>(lifeCycle);
            services.AddService<IMongoDriverRepository<TEntity>, TRepository>(lifeCycle);
        }

        public static void AddMongoRepository<TService, TEntity, TRepository>(this IServiceCollection services, LifeCycle lifeCycle = LifeCycle.SCOPED)
            where TService : class, IMongoDriverRepository<TEntity>
            where TEntity : class
            where TRepository : MongoDriverRepository<TEntity>, TService
        {

            services.AddService<IRepository<TEntity>, TRepository>(lifeCycle);
            services.AddService<IMongoDriverRepository<TEntity>, TRepository>(lifeCycle);
            services.AddService<TService, TRepository>(lifeCycle);
        }
    }
}
