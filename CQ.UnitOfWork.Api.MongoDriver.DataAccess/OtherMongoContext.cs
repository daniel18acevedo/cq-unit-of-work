using CQ.UnitOfWork.MongoDriver;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Api.MongoDriver.DataAccess
{
    public sealed class OtherMongoContext : MongoContext
    {
        public OtherMongoContext(IMongoDatabase mongoDatabase) : base(mongoDatabase, false) 
        {
            base.collections.Add(typeof(OtherUserMongo), "Users");
        }
    }
}
