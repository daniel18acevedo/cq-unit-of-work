using CQ.UnitOfWork.MongoDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Api.MongoDriver.DataAccess
{
    public class UserMongoRepository : MongoDriverRepository<UserMongo>
    {
        public UserMongoRepository(MongoContext context) : base(context) { }
    }
}
