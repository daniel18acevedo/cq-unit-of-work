using CQ.UnitOfWork.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Entities;

namespace CQ.UnitOfWork.Core
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        IRepository<TEntity> GetGenericRepository<TEntity>(Orms? orm=null) where TEntity : class;
    }
}
