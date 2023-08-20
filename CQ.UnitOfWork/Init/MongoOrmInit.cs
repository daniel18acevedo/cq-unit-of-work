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

namespace CQ.UnitOfWork.Init
{
    public static class MongoOrmInit
    {
        public static void AddMongoDriverOrm(this IServiceCollection services, MongoConfig mongoConfig)
        {
            if(mongoConfig is null)
            {
                throw new ArgumentNullException("mongoConfig");
            }
            mongoConfig.Assert();

            var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoConfig.DataBaseConnection.ConnectionString);
            mongoClientSettings.ClusterConfigurator = BuildClusterConfigurator(mongoConfig.ClusterConfigurator, mongoConfig.EnabledDefaultQueryLogger);

            var mongoClient = new MongoClient(mongoClientSettings);

            var mongoDatabase = mongoClient.GetDatabase(mongoConfig.DataBaseConnection.DatabaseName);

            // DataBaseConnection
            services.AddService(mongoConfig.LifeCycle, mongoDatabase);

            // For health check
            services.AddService<IDataBaseContext, MongoContext>(mongoConfig.LifeCycle);

            // Abstract mongo interface
            services.AddService<MongoContext>(mongoConfig.LifeCycle);
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

        public static void AddMongoContext<TContext>(this IServiceCollection services, LifeCycles lifeCycle, TContext context)
            where TContext : MongoContext
        {
            services.AddService<IDataBaseContext>(lifeCycle, context);

            services.AddService(lifeCycle, context);
        }

        public static void AddMongoRepository<T>(this IServiceCollection services, LifeCycles lifeCycle, string? collectionName = null) where T : class
        {
            services.AddService(lifeCycle, (serviceProvider) =>
            {
                var mongoContext = serviceProvider.GetService<MongoContext>();

                if (mongoContext is null)
                {
                    throw new ContextNotFoundException(Orms.MONGO_DB);
                }

                return new MongoRepository<T>(mongoContext, collectionName);
            });
        }
    }
}
