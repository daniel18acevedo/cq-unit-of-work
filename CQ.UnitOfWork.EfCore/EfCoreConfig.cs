using CQ.UnitOfWork.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.EfCore
{
    public class EfCoreConfig : OrmConfig
    {
        public Action<string>? Logger { get; set; }

        public EfCoreDataBaseEngines Engine { get; set; } = EfCoreDataBaseEngines.SQL;
    }
}
