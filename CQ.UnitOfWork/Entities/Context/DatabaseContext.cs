using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities.Context
{
    public class DatabaseContext
    {
        public Orms Orm { get; }

        public DatabaseContext(Orms orm) 
        { 
            this.Orm= orm;
        }
    }
}
