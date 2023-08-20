using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities.DataAccessConfig
{
    public class DatabaseConfig
    {

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public void Assert()
        {
            if (string.IsNullOrEmpty(this.ConnectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(this.DatabaseName))
            {
                throw new ArgumentNullException("databaseName");
            }
        }
    }
}
