using CQ.UnitOfWork.Core;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.IO;
using MongoDB.Driver;
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
        public static void AddMongoDatabase(this IServiceCollection services, string connectionString, string databaseName)
        {
            services.AddTransient((serviceProvider) =>
            {
                var mongoClient = new MongoClient(connectionString);

                var playerFinderDatabase = mongoClient.GetDatabase(databaseName);

                return playerFinderDatabase;
            });
        }

        public static void AddMongoDatabase(this IServiceCollection services, string host,int port, string databaseName, string applicationName= "MongoDB%20Compass")
        {
            services.AddTransient((serviceProvider) =>
            {
                var mongoClient = new MongoClient(new MongoClientSettings
                {
                    Server = new MongoServerAddress(host, port),
                    ReadPreference = ReadPreference.Primary,
                    ApplicationName = applicationName,
                    DirectConnection = true,
                });

                var playerFinderDatabase = mongoClient.GetDatabase(databaseName);

                return playerFinderDatabase;
            });
        }

        public static void AddMongoRepository<T>(this IServiceCollection services, string? collectionName = null) where T : class
        {
            services.AddTransient<IRepository<T>, MongoRepository<T>>((serviceProvider) =>
            {
                var mongoDatabase = serviceProvider.GetService<IMongoDatabase>();

                if(mongoDatabase is null)
                {
                    throw new Exception("Mongo database not initialized");
                }

                return new MongoRepository<T>(mongoDatabase, collectionName);
            });
        }

        public static void AddUnitOfWork(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWorkService>();
        }
    }
}
