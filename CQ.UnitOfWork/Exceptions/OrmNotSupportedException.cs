using CQ.UnitOfWork.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Exceptions
{
    public class OrmNotSupportedException : Exception
    {
        public Orms Orm { get; set; }

        public OrmNotSupportedException(Orms orm)  {

            Orm = orm;
        }
    }
}
