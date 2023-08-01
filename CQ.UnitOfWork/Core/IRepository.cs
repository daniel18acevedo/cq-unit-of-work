using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Core
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null);

        Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> expression);

        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// Get element with value in prop. By default the value is id
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        Task<TEntity> GetByPropAsync(string value, string? prop = null);

        Task<TEntity> CreateAsync(TEntity entity);

        Task DeleteAsync(Expression<Func<TEntity, bool>> expression);
    }
}
