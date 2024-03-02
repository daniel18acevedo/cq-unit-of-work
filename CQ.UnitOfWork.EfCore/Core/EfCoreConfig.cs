using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.EfCore
{
    public sealed record class EfCoreConfig : OrmConfig
    {
        public readonly Action<string>? Logger;

        public readonly EfCoreDataBaseEngine Engine;

        public EfCoreConfig(
            DatabaseConfig config,
            EfCoreDataBaseEngine engine = EfCoreDataBaseEngine.SQL,
            Action<string>? logger = null,
            bool useDefaultQueryLogger = false,
            bool @default = true)
            : base(
                  config,
                  logger == null && useDefaultQueryLogger,
                  @default)
        {
            this.Engine = engine;
            this.Logger = logger;
        }
    }
}
