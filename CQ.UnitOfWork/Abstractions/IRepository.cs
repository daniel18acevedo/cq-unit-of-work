using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class
    {
        #region Create entity
        /// <summary>
        /// Saves the entity pass as parameter into the data store with the ORM configured
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> CreateAsync(TEntity entity);

        TEntity Create(TEntity entity);
        #endregion

        #region Delete entity
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        void Delete(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region Fetch entities
        Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);

        IList<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null);

        Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null);
        
        IList<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null);
        #endregion

        #region Fetch entity
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity Get(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region Fetch entity by prop
        /// <summary>
        /// Get element with value in prop. By default the value is id
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        Task<TEntity> GetByPropAsync(string value, string? prop = null);

        TEntity GetByProp(string value, string? prop = null);
        #endregion

        #region Fetch entity or default
        Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate);
        #endregion

        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate);

        bool Exist(Expression<Func<TEntity, bool>> predicate);
    }
}
