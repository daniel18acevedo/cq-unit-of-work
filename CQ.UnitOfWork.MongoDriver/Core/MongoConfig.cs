using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.MongoDriver
{
    public class MongoConfig : OrmConfig
    {
        public Action<ClusterBuilder>? ClusterConfigurator { get; set; }
    }
}
