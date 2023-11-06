using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions
{
    public abstract class BaseRepository<TEntity> : IFetchRepository<TEntity>
        where TEntity : class
    {
        #region Abstractions
        public abstract TEntity Get(Expression<Func<TEntity, bool>> predicate);

        public abstract Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

        public abstract TEntity GetByProp(string value, string? prop = null);

        public abstract Task<TEntity> GetByPropAsync(string value, string? prop = null);

        public abstract TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate);

        public abstract Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        public abstract TEntity? GetOrDefaultByProp(string value, string? prop = null);

        public abstract Task<TEntity?> GetOrDefaultByPropAsync(string value, string? prop = null);
        #endregion


        public virtual TEntity? Get<TException>(Func<Expression<Func<TEntity, bool>>, TEntity?> function, Expression<Func<TEntity, bool>> predicate) where TException : Exception, new()
        {
            try
            {
                return function(predicate);
            }
            catch (Exception ex)
            {
                throw BuildCustomException<TException>(ex);
            }
        }

        public virtual async Task<TEntity?> GetAsync<TException>(Func<Expression<Func<TEntity, bool>>, Task<TEntity?>> function, Expression<Func<TEntity, bool>> predicate) where TException : Exception, new()
        {
            try
            {
                return await function(predicate).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw BuildCustomException<TException>(ex);
            }
        }

        public virtual TEntity? GetByProp<TException>(Func<string, string?, TEntity?> function, string value, string? prop = null) where TException : Exception, new()
        {
            try
            {
                return function(value, prop);
            }
            catch (Exception ex)
            {
                throw BuildCustomException<TException>(ex);
            }
        }

        public virtual async Task<TEntity?> GetByPropAsync<TException>(Func<string, string?, Task<TEntity?>> function, string value, string? prop = null) where TException : Exception, new()
        {
            try
            {
                return await function(value, prop).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw BuildCustomException<TException>(ex);
            }
        }

        private TException BuildCustomException<TException>(Exception origin)
            where TException : Exception, new()
        {
            var customException = new TException();

            customException.Data.Add("InnerException", origin);

            if (origin.InnerException?.Message.ToLower() == "sequence contains no elements")
            {
                customException.Data.Add("ErrorCode", "ResourceNotFound");
                customException.Data.Add("Message", $"{typeof(TEntity).Name} not found");
            }

            return customException;
        }
    }
}
