using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Core.Mongo
{
    public interface IMongoRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        Task UpdateByPropAsync(string value, object updates, string? prop = null);
    }
}
