﻿using CQ.UnitOfWork.Core;
using CQ.UnitOfWork.Entities;
using CQ.UnitOfWork.Exceptions;
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

namespace CQ.UnitOfWork.Init
{
    public static class UnitOfWorkInit
    {
        public static void AddMongoDatabase(this IServiceCollection services, string connectionString, string databaseName, Action<ClusterBuilder>? clusterConfigurator = null, bool useDefaultClusterConfigurator = false)
        {
            services.AddTransient((serviceProvider) =>
            {
                var mongoClientSettings = MongoClientSettings.FromConnectionString(connectionString);
                mongoClientSettings.ClusterConfigurator = BuildClusterConfigurator(clusterConfigurator, useDefaultClusterConfigurator);

                var mongoClient = new MongoClient(mongoClientSettings);

                var playerFinderDatabase = mongoClient.GetDatabase(databaseName);

                return playerFinderDatabase;
            });
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

        public static void AddMongoRepository<T>(this IServiceCollection services, string? collectionName = null) where T : class
        {
            services.AddTransient<IRepository<T>, MongoRepository<T>>((serviceProvider) =>
            {
                var mongoDatabase = serviceProvider.GetService<IMongoDatabase>();

                if (mongoDatabase is null)
                {
                    throw new DatabaseConnectionException(DataBaseEngines.MONGO);
                }

                return new MongoRepository<T>(mongoDatabase, collectionName);
            });
        }

        public static void AddUnitOfWork(this IServiceCollection services, Orms defaultOrm)
        {
            services.AddSingleton(typeof(Orms), defaultOrm);
            services.AddTransient<IUnitOfWork, UnitOfWorkService>();
        }
    }
}
