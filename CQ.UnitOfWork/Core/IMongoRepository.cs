using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Core
{
    public interface IMongoRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {

    }
}
