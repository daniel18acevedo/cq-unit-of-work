using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.MongoDriver.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.MongoDriver
{
    internal class MongoDriverRepository<TEntity> : Repository<TEntity>, IMongoDriverRepository<TEntity> where TEntity : class
    {
        protected readonly IMongoCollection<TEntity> _collection;
        protected readonly IMongoCollection<BsonDocument> _genericCollection;
        protected readonly string _collectionName;

        public MongoDriverRepository(MongoContext mongoContext, string? collectionName = null) : base(Orm.MONGO_DRIVER)
        {
            if (mongoContext is null)
            {
                throw new ArgumentNullException(nameof(mongoContext));
            }

            this._collectionName = mongoContext.BuildCollectionName<TEntity>(collectionName);

            this._collection = mongoContext.GetEntityCollection<TEntity>(collectionName);

            this._genericCollection = mongoContext.GetGenericCollection(collectionName);
        }

        public override async Task<TEntity> CreateAsync(TEntity entity)
        {
            await this._collection.InsertOneAsync(entity).ConfigureAwait(false);

            return entity;
        }

        public override TEntity Create(TEntity entity)
        {
            this._collection.InsertOne(entity);

            return entity;
        }

        public override async Task DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            var deleteResult = await this._collection.DeleteOneAsync(expression).ConfigureAwait(false);
        }

        public override void Delete(Expression<Func<TEntity, bool>> expression)
        {
            this._collection.DeleteOne(expression);
        }

        public override async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null)
        {
            expression ??= e => true;
            return await this._collection.Find(expression).ToListAsync().ConfigureAwait(false);
        }

        public override IList<TEntity> GetAll(Expression<Func<TEntity, bool>>? expression)
        {
            expression ??= e => true;

            return this._collection.Find(expression).ToList();
        }


        public override async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entity = await this.GetOrDefaultAsync(expression).ConfigureAwait(false);

            if (entity is null)
            {
                throw new ArgumentException($"{this._collectionName} not found");
            }

            return entity;
        }

        public override TEntity Get(Expression<Func<TEntity, bool>> expression)
        {
            var entity = this.GetOrDefault(expression);

            if (entity is null)
            {
                throw new ArgumentException($"{this._collectionName} not found");
            }

            return entity;
        }

        public override async Task<TEntity> GetByPropAsync(string value, string? prop = null)
        {
            prop ??= "_id";
            var filter = Builders<BsonDocument>.Filter.Eq(prop, value);

            var entity = await this._genericCollection.Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);

            if (entity is null)
            {
                throw new ArgumentException($"{this._collectionName} not found");
            }

            return BsonSerializer.Deserialize<TEntity>(entity);
        }

        public override TEntity GetByProp(string value, string? prop = "_id")
        {
            var filter = Builders<BsonDocument>.Filter.Eq(prop, value);

            var entity = this._genericCollection.Find(filter).FirstOrDefault();

            if (entity is null)
            {
                throw new ArgumentException($"{this._collectionName} not found");
            }

            return BsonSerializer.Deserialize<TEntity>(entity);
        }

        public override async Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entity = await this._collection.Find(expression).FirstOrDefaultAsync().ConfigureAwait(false);

            return entity;
        }

        public override TEntity? GetOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            return this._collection.Find(expression).FirstOrDefault();
        }

        public async Task UpdateByPropAsync(string value, object updates, string prop = "_id")
        {
            var (filter, updateDefinition) = BuilderFilterAndUpdateDefinition(value, updates, prop);

            var updateResult = await this._genericCollection.UpdateOneAsync(filter, updateDefinition).ConfigureAwait(false);
        }

        private (FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update) BuilderFilterAndUpdateDefinition(string value, object updates, string prop)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(prop, value);
            var updateBuilder = Builders<BsonDocument>.Update;

            var propertiesToUpdate = updates.GetType().GetProperties().ToList();
            var updateDefinitionList = new List<UpdateDefinition<BsonDocument>>();

            propertiesToUpdate.ForEach(property =>
            {
                var fieldName = property.Name;
                var fieldValue = property.GetValue(updates);
                var updateDefinition = updateBuilder.Set(fieldName, fieldValue);

                updateDefinitionList.Add(updateDefinition);
            });

            var updateDefinition = updateBuilder.Combine(updateDefinitionList);

            return (filter, updateDefinition);
        }

        public void UpdateByProp(string value, object updates, string prop = "_id")
        {
            var (filter, updateDefinition) = BuilderFilterAndUpdateDefinition(value, updates, prop);

            var updateResult = this._genericCollection.UpdateOne(filter, updateDefinition);
        }
    }
}
