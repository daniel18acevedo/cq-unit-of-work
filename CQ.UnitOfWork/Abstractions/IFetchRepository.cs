using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions
{
    public interface IFetchRepository<TEntity>
        where TEntity : class
    {
        #region Fetch entity
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetAsync<TException>(Expression<Func<TEntity, bool>> predicate, TException exception) where TException : Exception;

        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        TEntity Get<TException>(Expression<Func<TEntity, bool>> predicate, TException exception) where TException : Exception;
        #endregion

        #region Fetch entity or default
        Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate);
        #endregion


        #region Fetch entity by prop
        /// <summary>
        /// Get element with value in prop. By default the value is id
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        Task<TEntity> GetByPropAsync(string value, string? prop = null);

        Task<TEntity> GetByPropAsync<TException>(string value, TException exception, string? prop = null) where TException : Exception;

        TEntity GetByProp(string value, string? prop = null);

        TEntity GetByProp<TException>(string value, TException exception, string? prop = null) where TException : Exception;
        #endregion

        #region Fetch entity or default by prop
        Task<TEntity?> GetOrDefaultByPropAsync(string value, string? prop = null);

        TEntity? GetOrDefaultByProp(string value, string? prop = null);
        #endregion
    }
}
