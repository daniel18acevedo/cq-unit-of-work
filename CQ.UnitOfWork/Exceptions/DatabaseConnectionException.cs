using CQ.UnitOfWork.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Entities;

namespace CQ.UnitOfWork.Exceptions
{
    internal class DatabaseConnectionException : Exception
    {
        public DataBaseEngines Engine { get; set; }

        public DatabaseConnectionException(DataBaseEngines engine)
        {
            Engine = engine;
        }
    }
}
