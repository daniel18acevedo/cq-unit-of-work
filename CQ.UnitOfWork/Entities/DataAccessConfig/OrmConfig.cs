using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities.DataAccessConfig
{
    public abstract class OrmConfig
    {
        public DatabaseConfig DataBaseConnection { get; set; }

        public bool EnabledDefaultQueryLogger { get; set; }

        public void Assert()
        {
            if(this.DataBaseConnection is null)
            {
                throw new ArgumentNullException("dataBaseConnection");
            }

            this.DataBaseConnection.Assert();
        }
    }
}
