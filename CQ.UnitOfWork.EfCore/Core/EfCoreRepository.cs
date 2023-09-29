
using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.EfCore.Abstractions;
using CQ.UnitOfWork.EfCore.Extensions;
using CQ.UnitOfWork.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore
{
    public class EfCoreRepository<TEntity> : IEfCoreRepository<TEntity>, IUnitRepository<TEntity>
       where TEntity : class
    {
        protected DbSet<TEntity> _dbSet;

        protected EfCoreContext _efCoreConnection;

        protected string _tableName;

        public EfCoreRepository(EfCoreContext efCoreContext)
        {
            this.SetContext(efCoreContext);    
        }

        #region Create
        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await this._dbSet.AddAsync(entity).ConfigureAwait(false);

            await this._efCoreConnection.SaveChangesAsync().ConfigureAwait(false);

            return entity;
        }

        public virtual TEntity Create(TEntity entity)
        {
            this._dbSet.Add(entity);

            this._efCoreConnection.SaveChanges();

            return entity;
        }
        #endregion

        #region Delete
        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entitiesToRemove = this._dbSet.Where(predicate);

            await Task.Run(() => this._dbSet.RemoveRange(entitiesToRemove)).ConfigureAwait(false);

            await this._efCoreConnection.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual async void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entitiesToRemove = this._dbSet.Where(predicate);

            this._dbSet.RemoveRange(entitiesToRemove);

            this._efCoreConnection.SaveChanges();
        }
        #endregion

        #region GetAll
        public virtual async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return await this._dbSet.NullableWhere(predicate).ToListAsync().ConfigureAwait(false);
        }

        public virtual IList<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return this._dbSet.NullableWhere(predicate).ToList();
        }

        public virtual async Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null)
            where TResult : class
        {
            return await this._dbSet.NullableWhere(predicate).Select(selector).ToListAsync().ConfigureAwait(false);
        }

        public virtual IList<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null)
            where TResult : class
        {
            return this._dbSet.NullableWhere(predicate).Select(selector).ToList();
        }

        public virtual async Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null) 
        {
            return await this._dbSet.NullableWhere(predicate).SelectTo<TEntity, TResult>().ToListAsync().ConfigureAwait(false);
        }

        public virtual IList<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null) 
        {
            return this._dbSet.NullableWhere(predicate).SelectTo<TEntity, TResult>().ToList();
        }
        #endregion

        #region Get
        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = await this.GetOrDefaultAsync(predicate).ConfigureAwait(false);

            if (entity is null)
            {
                throw new ArgumentException($"{this._tableName} not found");
            }

            return entity;
        }

        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = this.GetOrDefault(predicate);

            if (entity is null)
            {
                throw new ArgumentException($"{this._tableName} not found");
            }

            return entity;
        }
        #endregion

        #region GetByProp
        public virtual async Task<TEntity> GetByPropAsync(string value, string? prop = null)
        {
            prop ??= "Id";
            var entity = await this._dbSet.FirstOrDefaultAsync(e => EF.Property<string>(e, prop) == value).ConfigureAwait(false);

            if (entity is null)
            {
                throw new ArgumentException($"{this._tableName} not found");
            }

            return entity;
        }

        public virtual TEntity GetByProp(string value, string? prop = "Id")
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
        public virtual async Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await this._dbSet.Where(predicate).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public virtual TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return this._dbSet.Where(predicate).FirstOrDefault();
        }
        #endregion

        #region Update
        public virtual async Task UpdateAsync(TEntity updated)
        {
            await Task.Run(() => this._dbSet.Update(updated)).ConfigureAwait(false);

            await this._efCoreConnection.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual void Update(TEntity updated)
        {
            this._dbSet.Update(updated);

            this._efCoreConnection.SaveChanges();
        }
        #endregion

        public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await this._dbSet.AnyAsync(predicate).ConfigureAwait(false);
        }

        public bool Exist(Expression<Func<TEntity, bool>> predicate)
        {
            return this._dbSet.Any(predicate);
        }

        public virtual void SetContext(IDatabaseContext context)
        {
            var efCoreContext = (EfCoreContext)context;

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.SetCollectionInfo(efCoreContext);
        }

        private void SetCollectionInfo(EfCoreContext efCoreContext)
        {
            this._dbSet = efCoreContext.GetEntitySet<TEntity>();
            this._tableName = efCoreContext.GetTableName<TEntity>();
            this._efCoreConnection = efCoreContext;
        }

        public virtual async Task CreateWithoutCommitAsync(TEntity entity)
        {
            await this._dbSet.AddAsync(entity).ConfigureAwait(false);
        }

        public virtual void CreateWithoutCommit(TEntity entity)
        {
            this._dbSet.Add(entity);
        }
    }
}