using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public readonly Orm Orm;

        public readonly string ConnectedTo;

        public Repository(Orm orm, string connectedTo)
        {
            this.Orm = orm;
            this.ConnectedTo = connectedTo;
        }

        public abstract TEntity Create(TEntity entity);

        public abstract Task<TEntity> CreateAsync(TEntity entity);

        public abstract void Delete(Expression<Func<TEntity, bool>> expression);

        public abstract Task DeleteAsync(Expression<Func<TEntity, bool>> expression);

        public abstract TEntity Get(Expression<Func<TEntity, bool>> expression);

        public abstract IList<TEntity> GetAll(Expression<Func<TEntity, bool>>? expression = null);

        public abstract Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null);

        public abstract Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression);

        public abstract TEntity GetByProp(string value, string? prop = null);

        public abstract Task<TEntity> GetByPropAsync(string value, string? prop = null);

        public abstract TEntity? GetOrDefault(Expression<Func<TEntity, bool>> expression);

        public abstract Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> expression);
    }
}
