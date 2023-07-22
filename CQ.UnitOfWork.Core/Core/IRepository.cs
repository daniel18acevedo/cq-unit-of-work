using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Core
{
    public interface IRepository<T> where T : class
    {
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? expression);

        Task<IEnumerable<U>> GetAllAsync<U>(Expression<Func<T, U>> selector, Expression<Func<T, bool>>? expression)
            where U : class;

        Task<T?> GetOrDefaultAsync(Expression<Func<T, bool>> expression);

        Task<T> GetAsync(Expression<Func<T, bool>> expression);

        Task<T> CreateAsync(T entity);

        Task DeleteAsync(Expression<Func<T, bool>> expression);
    }
}
