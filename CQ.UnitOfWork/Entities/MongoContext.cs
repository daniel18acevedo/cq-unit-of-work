using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities
{
    internal class MongoContext : DataBaseContext
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoContext(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public override bool Ping()
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
    }
}
