using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Abstractions.Extensions;
using CQ.UnitOfWork.MongoDriver.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.MongoDriver
{
    public static class MongoDriverInit
    {
        public static void AddMongoContext(this IServiceCollection services, MongoConfig config, LifeCycle contextLifeCycle = LifeCycle.SCOPED, LifeCycle mongoClientLifeCycle = LifeCycle.SINGLETON)
        {
            config.Assert();


            services.AddService<IMongoClient>((serviceProvider) =>
            {
                var mongoClientSettings = MongoClientSettings.FromConnectionString(config.DatabaseConnection.ConnectionString);
                mongoClientSettings.ClusterConfigurator = BuildClusterConfigurator(config.ClusterConfigurator, config.UseDefaultQueryLogger);

                var mongoClient = new MongoClient(mongoClientSettings);

                return mongoClient;
            }, mongoClientLifeCycle);

            services.AddService(serviceProvider =>
            {
                var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();

                var mongoDatabase = mongoClient.GetDatabase(config.DatabaseConnection.DatabaseName);

                return new MongoContext(mongoDatabase);
            }, contextLifeCycle);
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

            services.AddService<Repository<TEntity>>(implementationFactory, lifeCycle);
            services.AddService<IRepository<TEntity>>(implementationFactory, lifeCycle);
            services.AddService<IMongoDriverRepository<TEntity>>(implementationFactory, lifeCycle);
        }
    }
}
