using CQ.UnitOfWork.Entities.Context;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities
{
    public class MongoContext : DatabaseContext, IDataBaseContext
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoContext(IMongoDatabase mongoDatabase) : base(Orms.MONGO_DB)
        {
            _mongoDatabase = mongoDatabase;
        }

        public bool Ping()
        {
            try
            {
                var result = this._mongoDatabase.RunCommand<BsonDocument>(new BsonDocument("ping", 1));

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
