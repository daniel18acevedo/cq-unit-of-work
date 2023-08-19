using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities.DataAccessConfig
{
    public class DataBaseConnection
    {

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public bool EnabledDefaultQueryLogger { get; set; }
    }
}
