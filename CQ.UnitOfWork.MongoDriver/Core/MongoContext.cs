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

        private List<Action> _actions = new List<Action>();

        private List<Func<Task>> _actionsTask = new List<Func<Task>>();

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
            return collectionName?? $"{typeof(TEntity).Name}s";
        }

        public IMongoCollection<BsonDocument> GetGenericCollection(string? collectionName)
        {
            return GetEntityCollection<BsonDocument>(collectionName);
        }

        public void AddActionAsync(Func<Task> action)
        {
            this._actionsTask.Add(action);
        }

        public void AddAction(Action action)
        {
            this._actions.Add(action);
        }

        public async Task SaveChangesAsync()
        {
            this._actions.ForEach(action =>
            {
                action();
            });

            Parallel.ForEach(this._actionsTask, async action =>
            {
                await action().ConfigureAwait(false);
            });
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
