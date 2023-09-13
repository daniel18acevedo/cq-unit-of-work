using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions
{
    public enum LifeCycle
    {
        SCOPED,
        TRANSIENT,
        SINGLETON
    }
}
