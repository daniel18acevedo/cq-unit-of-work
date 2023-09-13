using CQ.UnitOfWork.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.MongoDriver
{
    public class MongoContext : IDatabaseContext
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoContext(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public bool Ping(string? collection = null)
        {
            try
            {
                var result = this._mongoDatabase.RunCommand<BsonDocument>(new BsonDocument($"{collection ?? "ping"}", 1));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IMongoCollection<TEntity> GetEntityCollection<TEntity>(string? collectionName)
        {
            collectionName = this.BuildCollectionName<TEntity>(collectionName);
            var collection = this._mongoDatabase.GetCollection<TEntity>(collectionName);

            return collection;
        }

        public string BuildCollectionName<TEntity>(string? collectionName)
        {
            return string.IsNullOrEmpty(collectionName) ? $"{typeof(TEntity).Name}s" : collectionName;
        }

        public IMongoCollection<BsonDocument> GetGenericCollection(string? collectionName)
        {
            return GetEntityCollection<BsonDocument>(collectionName);
        }
    }
}
