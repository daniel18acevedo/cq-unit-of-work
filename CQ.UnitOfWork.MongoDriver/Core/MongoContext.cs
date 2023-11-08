using CQ.UnitOfWork.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.MongoDriver
{
    public class MongoContext : IDatabaseContext
    {
        private readonly IMongoDatabase _mongoDatabase;

        private readonly List<Action> _actions = new ();

        private readonly List<Func<Task>> _actionsTask = new ();

        protected readonly IDictionary<Type, string> collections = new Dictionary<Type, string>();

        public readonly bool IsDefault;

        public MongoContext(IMongoDatabase mongoDatabase, bool isDefault)
        {
            _mongoDatabase = mongoDatabase;
            IsDefault = isDefault;
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

        public IMongoCollection<TEntity> GetEntityCollection<TEntity>()
        {
            var collectionName = this.GetCollectionName<TEntity>();
         
            return this._mongoDatabase.GetCollection<TEntity>(collectionName);
        }

        public bool HasCollectionRegistered<TEntity>()
        {
            return this.collections.ContainsKey(typeof(TEntity));
        }

        public string GetCollectionName<TEntity>()
        {
            this.collections.TryGetValue(typeof(TEntity), out string? collectionName);

            return collectionName ?? $"{typeof(TEntity).Name}s";
        }

        public IMongoCollection<BsonDocument> GetGenericCollection<TEntity>()
        {
            var collectionName = this.GetCollectionName<TEntity>();

            return this._mongoDatabase.GetCollection<BsonDocument>(collectionName);
        }

        public void AddActionAsync(Func<Task> action)
        {
            this._actionsTask.Add(action);
        }

        public void AddAction(Action action)
        {
            this._actions.Add(action);
        }

        public Task SaveChangesAsync()
        {
            this._actions.ForEach(action =>
            {
                action();
            });

            Parallel.ForEach(this._actionsTask, async action =>
            {
                await action().ConfigureAwait(false);
            });

            return Task.CompletedTask;
        }

        public DatabaseInfo GetDatabaseInfo()
        {
            return new DatabaseInfo
            {
                Provider = "Mongo",
                Name = this._mongoDatabase.DatabaseNamespace.DatabaseName
            };
        }
    }
}
