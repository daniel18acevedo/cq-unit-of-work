using CQ.UnitOfWork.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Entities;

namespace CQ.UnitOfWork.Exceptions
{
    internal class ContextNotFoundException : Exception
    {
        public Orms Orm { get; set; }

        public ContextNotFoundException(Orms orm)
        {
            this.Orm = orm;
        }
    }
}
