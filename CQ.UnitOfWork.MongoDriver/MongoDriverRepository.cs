using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.MongoDriver.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.MongoDriver
{
    public class MongoDriverRepository<TEntity> : IMongoDriverRepository<TEntity>, IUnitRepository<TEntity> where TEntity : class
    {
        protected MongoContext _mongoContext;
        protected IMongoCollection<TEntity> _collection;
        protected IMongoCollection<BsonDocument> _genericCollection;
        protected string _collectionName;

        public MongoDriverRepository(MongoContext mongoContext, string? collectionName = null)
        {
            this._collectionName = collectionName;

            this.SetContext(mongoContext);
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await this._collection.InsertOneAsync(entity).ConfigureAwait(false);

            return entity;
        }

        public virtual TEntity Create(TEntity entity)
        {
            this._collection.InsertOne(entity);

            return entity;
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            var deleteResult = await this._collection.DeleteOneAsync(expression).ConfigureAwait(false);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> expression)
        {
            this._collection.DeleteOne(expression);
        }

        public virtual async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null)
        {
            expression ??= e => true;
            return await this._collection.Find(expression).ToListAsync().ConfigureAwait(false);
        }

        public virtual IList<TEntity> GetAll(Expression<Func<TEntity, bool>>? expression)
        {
            expression ??= e => true;

            return this._collection.Find(expression).ToList();
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entity = await this.GetOrDefaultAsync(expression).ConfigureAwait(false);

            if (entity is null)
            {
                throw new ArgumentException($"{this._collectionName} not found");
            }

            return entity;
        }

        public virtual TEntity Get(Expression<Func<TEntity, bool>> expression)
        {
            var entity = this.GetOrDefault(expression);

            if (entity is null)
            {
                throw new ArgumentException($"{this._collectionName} not found");
            }

            return entity;
        }

        public virtual async Task<TEntity> GetByPropAsync(string value, string? prop = null)
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

        public virtual TEntity GetByProp(string value, string? prop = "_id")
        {
            var filter = Builders<BsonDocument>.Filter.Eq(prop, value);

            var entity = this._genericCollection.Find(filter).FirstOrDefault();

            if (entity is null)
            {
                throw new ArgumentException($"{this._collectionName} not found");
            }

            return BsonSerializer.Deserialize<TEntity>(entity);
        }

        public virtual async Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entity = await this._collection.Find(expression).FirstOrDefaultAsync().ConfigureAwait(false);

            return entity;
        }

        public virtual TEntity? GetOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            return this._collection.Find(expression).FirstOrDefault();
        }

        public virtual async Task UpdateByPropAsync(string value, object updates, string prop = "_id")
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

        public virtual void UpdateByProp(string value, object updates, string prop = "_id")
        {
            var (filter, updateDefinition) = BuilderFilterAndUpdateDefinition(value, updates, prop);

            var updateResult = this._genericCollection.UpdateOne(filter, updateDefinition);
        }

        public virtual void SetContext(IDatabaseContext context)
        {
            var mongoContext = (MongoContext)context;
            if (context is null)
            {
                throw new ArgumentNullException(nameof(mongoContext));
            }
            this._mongoContext = mongoContext;

            this._collectionName = mongoContext.BuildCollectionName<TEntity>(this._collectionName);

            this._collection = mongoContext.GetEntityCollection<TEntity>(this._collectionName);

            this._genericCollection = mongoContext.GetGenericCollection(this._collectionName);
        }

        public virtual async Task CreateWithoutCommitAsync(TEntity entity)
        {
            await Task.Run(() => this._mongoContext.AddActionAsync(async () => await this._collection.InsertOneAsync(entity).ConfigureAwait(false))).ConfigureAwait(false);
        }

        public virtual void CreateWithoutCommit(TEntity entity)
        {
            this._mongoContext.AddAction(() => this._collection.InsertOne(entity));
        }
    }
}
