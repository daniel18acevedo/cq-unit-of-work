
using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Abstractions.Extensions;
using CQ.UnitOfWork.EfCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore
{
    public class EfCoreRepository<TEntity> : IEfCoreRepository<TEntity>
       where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet;

        private readonly EfCoreContext _efCoreConnection;

        private readonly string _tableName;

        public EfCoreRepository(EfCoreContext efCoreContext)
        {
            if (efCoreContext is null)
            {
                throw new ArgumentNullException(nameof(efCoreContext));
            }

            this._dbSet = efCoreContext.GetEntitySet<TEntity>();
            this._tableName = efCoreContext.GetTableName<TEntity>();
            this._efCoreConnection = efCoreContext;
        }

        #region Create
        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await this._dbSet.AddAsync(entity).ConfigureAwait(false);

            await this._efCoreConnection.SaveChangesAsync().ConfigureAwait(false);

            return entity;
        }

        public TEntity Create(TEntity entity)
        {
            this._dbSet.Add(entity);

            this._efCoreConnection.SaveChanges();

            return entity;
        }
        #endregion

        #region Delete
        public async Task DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entitiesToRemove = this._dbSet.Where(expression);

            await Task.Run(() => this._dbSet.RemoveRange(entitiesToRemove)).ConfigureAwait(false);

            await this._efCoreConnection.SaveChangesAsync().ConfigureAwait(false);
        }

        public async void Delete(Expression<Func<TEntity, bool>> expression)
        {
            var entitiesToRemove = this._dbSet.Where(expression);

            this._dbSet.RemoveRange(entitiesToRemove);

            this._efCoreConnection.SaveChanges();
        }
        #endregion

        #region GetAll
        public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null)
        {
            return await this._dbSet.NullableWhere(expression).ToListAsync().ConfigureAwait(false);
        }

        public IList<TEntity> GetAll(Expression<Func<TEntity, bool>>? expression = null)
        {
            return this._dbSet.NullableWhere(expression).ToList();
        }

        public async Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? expression = null)
            where TResult : class
        {
            return await this._dbSet.NullableWhere(expression).Select(selector).ToListAsync().ConfigureAwait(false);
        }

        public IList<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? expression = null)
            where TResult : class
        {
            return this._dbSet.NullableWhere(expression).Select(selector).ToList();
        }
        #endregion

        #region Get
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entity = await this.GetOrDefaultAsync(expression).ConfigureAwait(false);

            if (entity is null)
            {
                throw new ArgumentException($"{this._tableName} not found");
            }

            return entity;
        }

        public TEntity Get(Expression<Func<TEntity, bool>> expression)
        {
            var entity = this.GetOrDefault(expression);

            if (entity is null)
            {
                throw new ArgumentException($"{this._tableName} not found");
            }

            return entity;
        }
        #endregion

        #region GetByProp
        public async Task<TEntity> GetByPropAsync(string value, string? prop = null)
        {
            prop ??= "Id";
            var entity = await this._dbSet.FirstOrDefaultAsync(e => EF.Property<string>(e, prop) == value).ConfigureAwait(false);

            if (entity is null)
            {
                throw new ArgumentException($"{this._tableName} not found");
            }

            return entity;
        }

        public TEntity GetByProp(string value, string? prop = "Id")
        {
            var entity = this._dbSet.FirstOrDefault(e => EF.Property<string>(e, prop) == value);

            if (entity is null)
            {
                throw new ArgumentException($"{this._tableName} not found");
            }

            return entity;
        }
        #endregion

        #region GetOrDefault
        public async Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await this._dbSet.Where(expression).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public TEntity? GetOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            return this._dbSet.Where(expression).FirstOrDefault();
        }
        #endregion

        #region Update
        public async Task UpdateAsync(TEntity updated)
        {
            await Task.Run(() => this._dbSet.Update(updated)).ConfigureAwait(false);

            await this._efCoreConnection.SaveChangesAsync().ConfigureAwait(false);
        }

        public void Update(TEntity updated)
        {
            this._dbSet.Update(updated);

            this._efCoreConnection.SaveChanges();
        }
        #endregion
    }
}