using CQ.UnitOfWork.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions
{
    public abstract class BaseRepository<TEntity>
        : IFetchRepository<TEntity>
        where TEntity : class
    {
        protected string EntityName => typeof(TEntity).Name;

        #region Abstractions
        public abstract TEntity Get(Expression<Func<TEntity, bool>> predicate);

        public abstract Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

        public abstract TEntity GetByProp(string value, string prop);

        public abstract Task<TEntity> GetByPropAsync(string value, string prop);

        public abstract TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate);

        public abstract Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        public abstract TEntity? GetOrDefaultByProp(string value, string prop);

        public abstract Task<TEntity?> GetOrDefaultByPropAsync(string value, string prop);

        public abstract Task<TEntity> GetByIdAsync(string id);

        public abstract TEntity GetById(string id);

        public abstract Task<TEntity?> GetOrDefaultByIdAsync(string id);

        public abstract TEntity? GetOrDefaultById(string id);
        #endregion

        public virtual async Task<TEntity> GetAsync<TException>(Expression<Func<TEntity, bool>> predicate, TException exception)
            where TException : Exception
        {
            try
            {
                return await this.GetAsync(predicate).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exception.SetInnerException(ex);
                throw exception;
            }
        }

        public virtual TEntity Get<TException>(Expression<Func<TEntity, bool>> predicate, TException exception)
            where TException : Exception
        {
            try
            {
                return this.Get(predicate);
            }
            catch (Exception ex)
            {
                exception.SetInnerException(ex);
                throw exception;
            }
        }

        public virtual async Task<TEntity> GetByPropAsync<TException>(string value, string prop, TException exception) where TException : Exception
        {
            try
            {
                return await this.GetByPropAsync(value, prop).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exception.SetInnerException(ex);
                throw exception;
            }
        }

        public virtual TEntity GetByProp<TException>(string value, string prop, TException exception) where TException : Exception
        {
            try
            {
                return this.GetByProp(value, prop);
            }
            catch (Exception ex)
            {
                exception.SetInnerException(ex);
                throw exception;
            }
        }

        public virtual async Task<TEntity> GetByIdAsync<TException>(string id, TException exception) where TException : Exception
        {
            try
            {
                return await this.GetByIdAsync(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exception.SetInnerException(ex);
                throw exception;
            }
        }

        public TEntity GetById<TException>(string id, TException exception) where TException : Exception
        {
            try
            {
                return this.GetById(id);
            }
            catch (Exception ex)
            {
                exception.SetInnerException(ex);
                throw exception;
            }
        }
    }
}
