using CQ.UnitOfWork.Entities.DataAccessConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities.ServiceConfig
{
    public class OrmServiceConfig<TOrmConfig>
        where TOrmConfig : class
    {
        public LifeCycles LifeCycle { get; set; }

        public TOrmConfig Config { get; set; }
    }
}
