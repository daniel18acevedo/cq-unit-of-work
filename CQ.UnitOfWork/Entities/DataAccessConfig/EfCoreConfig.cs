using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities.DataAccessConfig
{
    public class EfCoreConfig : OrmConfig
    {
        public Action<string>? Logger { get; set; }

        public EfCoreDataBaseEngines Engine { get; set; }

        public EfCoreConfig() : base(Orms.EF_CORE)
        {
            Engine = EfCoreDataBaseEngines.SQL;
        }
    }
}
