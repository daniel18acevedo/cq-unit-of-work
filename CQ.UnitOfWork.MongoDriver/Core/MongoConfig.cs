using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.MongoDriver
{
    public sealed record class MongoConfig : OrmConfig
    {
        public readonly Action<ClusterBuilder>? ClusterConfigurator;

        public MongoConfig(
            DatabaseConfig databaseConnection,
            bool useDefaultQueryLogger = false,
            bool @default = true,
            Action<ClusterBuilder>? clusterConfigurator = null)
            : base(
                  databaseConnection,
                  clusterConfigurator == null && useDefaultQueryLogger,
                  @default)
        {
            this.ClusterConfigurator = clusterConfigurator;
        }
    }
}
