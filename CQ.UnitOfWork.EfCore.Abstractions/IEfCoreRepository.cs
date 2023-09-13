using CQ.UnitOfWork.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.EfCore.Abstractions
{
    public interface IEfCoreRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? expression = null)
        where TResult : class;

        IList<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? expression = null)
            where TResult : class;

        Task UpdateAsync(TEntity updated);

        void Update(TEntity updated);
    }
}
