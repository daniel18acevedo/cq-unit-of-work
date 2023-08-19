using CQ.UnitOfWork.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Core
{
    public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class
    {
        private readonly IMongoCollection<TEntity> _collection;
        private readonly IMongoCollection<BsonDocument> _genericCollection;
        private readonly string _collectionName;

        public MongoRepository(MongoContext mongoContext, string? collectionName=null)
        {
            this._collectionName = mongoContext.BuildCollectionName<TEntity>(collectionName);

            this._collection = mongoContext.GetEntityCollection<TEntity>(collectionName);
            
            this._genericCollection = mongoContext.GetGenericCollection(collectionName);
        }

        public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null)
        {
            expression ??= e => true;
            var entities = await this._collection.Find(expression).ToListAsync().ConfigureAwait(false);

            return entities;
        }

        public async Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entity = await this._collection.Find(expression).FirstOrDefaultAsync().ConfigureAwait(false);

            return entity;
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entity = await this.GetOrDefaultAsync(expression).ConfigureAwait(false);

            if (entity is null)
            {
                throw new Exception($"{this._collectionName} not found");
            }

            return entity;
        }

        public async Task<TEntity> GetByPropAsync(string value, string? prop = "_id")
        {
            var filter = Builders<BsonDocument>.Filter.Eq(prop, value);

            var entity = await this._genericCollection.Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);

            if (entity is null)
            {
                throw new Exception($"{this._collectionName} not found");
            }

            return BsonSerializer.Deserialize<TEntity>(entity);
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await this._collection.InsertOneAsync(entity).ConfigureAwait(false);

            return entity;
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            var deleteResult = await this._collection.DeleteOneAsync(expression).ConfigureAwait(false);
        }
    }
}
