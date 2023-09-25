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

        public EfCoreDataBaseEngine Engine { get; set; } = EfCoreDataBaseEngine.SQL;

        public EfCoreConfig() : base(Orm.EF_CORE) { }
    }
}
