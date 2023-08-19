using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities.DataAccessConfig
{
    public abstract class OrmConfig
    {
        public DataBaseConnection DataBaseConnection { get; set; }

        public bool EnabledDefaultQueryLogger { get; set; }

        public LifeCycles LifeCycle { get; set; }  

        public Orms Orm { get; }

        public OrmConfig(Orms orm)
        {
            Orm = orm;
        }
    }
}
