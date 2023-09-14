using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions
{
    public abstract class OrmConfig
    {
        public DatabaseConfig DatabaseConnection { get; set; }

        public bool UseDefaultQueryLogger { get; set; }

        public Orm Orm { get; set; }

        public OrmConfig(Orm orm)
        {
            this.Orm = orm;
        }

        public void Assert()
        {
            if(this.DatabaseConnection is null)
            {
                throw new ArgumentNullException("dataBaseConnection");
            }

            this.DatabaseConnection.Assert();
        }
    }
}
