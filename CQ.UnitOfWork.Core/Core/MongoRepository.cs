using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Core
{
    public class MongoRepository<T> : IRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;
        private readonly string CollectionName;

        public MongoRepository(IMongoDatabase mongoDatabase, string? collectionName = null)
        {
            this.CollectionName = string.IsNullOrEmpty(collectionName) ? $"{typeof(T).Name}s" : collectionName;

            this._collection = mongoDatabase.GetCollection<T>(this.CollectionName);
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? expression)
        {
            var entities = await this._collection.Find(expression).ToListAsync().ConfigureAwait(false);
            
            return entities;
        }

        public async Task<IEnumerable<U>> GetAllAsync<U>(Expression<Func<T, U>> selector, Expression<Func<T, bool>>? expression)
            where U : class
        {
            var entities = await this._collection.Find(expression).As<U>().ToListAsync();

            return entities;
        }

        public async Task<T?> GetOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            var entity = await this._collection.Find(expression).FirstOrDefaultAsync().ConfigureAwait(false);

            return entity;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> expression)
        {
            var entity = await this.GetOrDefaultAsync(expression).ConfigureAwait(false);

            if(entity is null)
            {
                throw new Exception($"{this.CollectionName} not found");
            }

            return entity;
        }

        public async Task<T> CreateAsync(T entity)
        {
            await this._collection.InsertOneAsync(entity).ConfigureAwait(false);

            return entity;
        }

        public async Task DeleteAsync(Expression<Func<T,bool>> expression)
        {
            var deleteResult = await this._collection.DeleteOneAsync(expression).ConfigureAwait(false);
            
            if(deleteResult is null)
            {
                return;
            }

            if(deleteResult.DeletedCount == 0)
            {
                throw new Exception("None elements were deleted");
            }
        }

    }
}
