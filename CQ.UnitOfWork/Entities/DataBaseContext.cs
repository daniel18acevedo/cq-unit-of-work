using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities
{
    public abstract class DataBaseContext
    {
        public abstract bool Ping();
    }
}
