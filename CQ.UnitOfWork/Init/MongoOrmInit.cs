using CQ.UnitOfWork.Entities.DataAccessConfig;
using CQ.UnitOfWork.Entities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using CQ.UnitOfWork.Exceptions;
using CQ.UnitOfWork.Core;
using Amazon.Util;
using CQ.UnitOfWork.Entities.Context;
using CQ.UnitOfWork.Core.Mongo;

namespace CQ.UnitOfWork.Init
{
    public static class MongoOrmInit
    {
        public static void AddMongoDriverOrm(this IServiceCollection services, LifeCycles lifeCycle, MongoConfig mongoConfig)
        {
            if (mongoConfig is null)
            {
                throw new ArgumentNullException("mongoConfig");
            }
            mongoConfig.Assert();

            var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoConfig.DataBaseConnection.ConnectionString);
            mongoClientSettings.ClusterConfigurator = BuildClusterConfigurator(mongoConfig.ClusterConfigurator, mongoConfig.EnabledDefaultQueryLogger);

            var mongoClient = new MongoClient(mongoClientSettings);

            var mongoDatabase = mongoClient.GetDatabase(mongoConfig.DataBaseConnection.DatabaseName);

            // DataBaseConnection
            services.AddService(lifeCycle, mongoDatabase);

            services.AddMongoConnection(lifeCycle);
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

        public static void AddMongoConnection(this IServiceCollection services, LifeCycles lifeCycle)
        {
            // For ping
            services.AddService<IDatabaseConnection, MongoConnection>(lifeCycle);

            // For build specific orm to repository
            services.AddService<MongoConnection>(lifeCycle);
        }

        public static void AddMongoRepository<TEntity>(this IServiceCollection services, LifeCycles lifeCycle, string? collectionName = null) 
            where TEntity : class
        {
            var implementationFactory = (IServiceProvider serviceProvider) =>
            {
                var mongoContext = serviceProvider.GetService<MongoConnection>();

                if (mongoContext is null)
                {
                    throw new ContextNotFoundException(Orms.MONGO_DB);
                }

                return new MongoRepository<TEntity>(mongoContext, collectionName);
            };

            services.AddService<IRepository<TEntity>, MongoRepository<TEntity>>(lifeCycle, implementationFactory);

            services.AddService<IMongoRepository<TEntity>, MongoRepository<TEntity>>(lifeCycle, implementationFactory);
        }
    }
}
