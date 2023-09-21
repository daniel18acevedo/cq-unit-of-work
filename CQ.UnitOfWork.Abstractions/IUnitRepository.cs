using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions
{
    public interface IUnitRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        void SetContext(IDatabaseContext context);
    }
}
