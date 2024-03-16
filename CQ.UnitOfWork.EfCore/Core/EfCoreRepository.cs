
using CQ.Exceptions;
using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.EfCore.Abstractions;
using CQ.UnitOfWork.EfCore.Extensions;
using CQ.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.AccessControl;

namespace CQ.UnitOfWork.EfCore
{
    public class EfCoreRepository<TEntity> :
        BaseRepository<TEntity>,
        IEfCoreRepository<TEntity>,
        IUnitRepository<TEntity>
       where TEntity : class
    {
        protected DbSet<TEntity> _dbSet = null!;

        protected EfCoreContext _efCoreConnection = null!;

        public EfCoreRepository(EfCoreContext efCoreContext)
        {
            this.SetContext(efCoreContext);
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
            this._efCoreConnection = efCoreContext;
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

        public virtual async Task<List<TEntity>> CreateBulkAsync(List<TEntity> entities)
        {
            await this._dbSet.AddRangeAsync(entities).ConfigureAwait(false);

            await this._efCoreConnection.SaveChangesAsync().ConfigureAwait(false);

            return entities;
        }

        public virtual List<TEntity> CreateBulk(List<TEntity> entities)
        {
            this._dbSet.AddRange(entities);

            this._efCoreConnection.SaveChanges();

            return entities;
        }

        #endregion

        #region Delete
        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entitiesToRemove = this._dbSet.Where(predicate);

            this._dbSet.RemoveRange(entitiesToRemove);

            await this._efCoreConnection.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entitiesToRemove = this._dbSet.Where(predicate);

            this._dbSet.RemoveRange(entitiesToRemove);

            this._efCoreConnection.SaveChanges();
        }
        #endregion

        #region GetAll
        public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return await this._dbSet
                .NullableWhere(predicate)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public virtual List<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return this._dbSet.NullableWhere(predicate).ToList();
        }

        public virtual async Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null)
            where TResult : class
        {
            return await this._dbSet.NullableWhere(predicate).Select(selector).ToListAsync().ConfigureAwait(false);
        }

        public virtual List<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null)
            where TResult : class
        {
            return this._dbSet.NullableWhere(predicate).Select(selector).ToList();
        }

        public virtual async Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return await this._dbSet.NullableWhere(predicate).SelectTo<TEntity, TResult>().ToListAsync().ConfigureAwait(false);
        }

        public virtual List<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return this._dbSet.NullableWhere(predicate).SelectTo<TEntity, TResult>().ToList();
        }
        #endregion

        #region GetPaginated
        public virtual async Task<Pagination<TEntity>> GetPagedAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            int page = 1,
            int pageSize = 10)
        {
            var itemsPaged = await this._dbSet
                .NullableWhere(predicate)
                .Paginate(page, pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            var totalItems = await this._dbSet
                .NullableCountAsync(predicate)
                .ConfigureAwait(false);

            double itemsPerPage = pageSize == 0 ? totalItems : pageSize;
            var totalPages = Convert.ToInt64(Math.Ceiling(totalItems / itemsPerPage));

            return new Pagination<TEntity>(
                itemsPaged,
                totalItems,
                totalPages);
        }

        public virtual Pagination<TEntity> GetPaged(
            Expression<Func<TEntity, bool>>? predicate = null,
            int page = 1,
            int pageSize = 10)
        {
            var itemsPaged = this._dbSet
                .NullableWhere(predicate)
                .Paginate(page, pageSize)
                .ToList();

            var totalItems = this._dbSet
                .NullableCount(predicate);

            double itemsPerPage = pageSize == 0 ? totalItems : pageSize;
            var totalPages = Convert.ToInt64(Math.Ceiling(totalItems / itemsPerPage));

            return new Pagination<TEntity>(
                itemsPaged,
                totalItems,
                totalPages);
        }
        #endregion

        #region Get
        public override async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = await this.GetOrDefaultAsync(predicate).ConfigureAwait(false);

            if (Guard.IsNull(entity))
                throw new SpecificResourceNotFoundException<TEntity>("condition", string.Empty);

            return entity;
        }

        public override TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = this.GetOrDefault(predicate);

            if (Guard.IsNull(entity))
                throw new SpecificResourceNotFoundException<TEntity>("condition", string.Empty);

            return entity;
        }
        #endregion

        #region GetByProp
        public override async Task<TEntity> GetByPropAsync(string value, string prop)
        {
            var entity = await this.GetOrDefaultByPropAsync(value, prop).ConfigureAwait(false);

            if (Guard.IsNull(entity))
                throw new SpecificResourceNotFoundException<TEntity>(prop, value);

            return entity;
        }

        public override TEntity GetByProp(string value, string prop)
        {
            var entity = this.GetOrDefaultByProp(value, prop);

            if (Guard.IsNull(entity))
                throw new SpecificResourceNotFoundException<TEntity>(prop, value);

            return entity;
        }
        #endregion

        #region GetOrDefault
        public override async Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await this._dbSet.Where(predicate).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public override TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return this._dbSet.Where(predicate).FirstOrDefault();
        }
        #endregion

        #region GetOrDefaultByProp
        public override async Task<TEntity?> GetOrDefaultByPropAsync(string value, string prop)
        {
            var entity = await this.GetOrDefaultAsync(e => EF.Property<string>(e, prop) == value).ConfigureAwait(false);

            return entity;
        }

        public override TEntity? GetOrDefaultByProp(string value, string prop)
        {
            var entity = this.GetOrDefault(e => EF.Property<string>(e, prop) == value);

            return entity;
        }
        #endregion

        #region GetById

        public override async Task<TEntity> GetByIdAsync(string id)
        {
            return await this.GetByPropAsync(id, "Id").ConfigureAwait(false);
        }

        public override TEntity GetById(string id)
        {
            return this.GetByProp(id, "Id");
        }

        public override async Task<TEntity?> GetOrDefaultByIdAsync(string id)
        {
            return await this.GetOrDefaultByPropAsync(id, "Id").ConfigureAwait(false);
        }

        public override TEntity? GetOrDefaultById(string id)
        {
            return this.GetOrDefaultByProp(id, "Id");
        }
        #endregion

        #region Update
        public virtual async Task UpdateAsync(TEntity updated)
        {
            this._dbSet.Update(updated);

            await this._efCoreConnection.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual void Update(TEntity updated)
        {
            this._dbSet.Update(updated);

            this._efCoreConnection.SaveChanges();
        }

        public virtual async Task UpdateByIdAsync(string id, object updates)
        {
            await UpdateByPropAsync(id, updates, "Id").ConfigureAwait(false);

            await this._efCoreConnection.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual void UpdateById(string id, object updates)
        {
            UpdateByProp(id, updates, "Id");
        }

        public virtual async Task UpdateByPropAsync(string value, object updates, string prop)
        {
            var query = this.BuildUpdateQuery(updates, prop, value);

            var rawsAffected = await this._efCoreConnection.Database.ExecuteSqlRawAsync(query).ConfigureAwait(false);

            if (rawsAffected != 0)
            {
                var entity = this._dbSet.Find(value)!;
                await this._efCoreConnection.Entry(entity).ReloadAsync().ConfigureAwait(false);
            }
        }

        private string BuildUpdateQuery(object updates, string id, string idValue)
        {
            var typeofUpdates = updates.GetType();
            var propsOfUpdates = typeofUpdates.GetProperties();
            var namesOfProps = propsOfUpdates.Select(p =>
            {
                var propertyType = p.PropertyType.Name.ToLower();
                var value = p.GetValue(updates);

                if (propertyType == "string")
                {
                    return $"{p.Name}='{value}'";
                }

                return $"{p.Name}={value}";
            });
            var updatesSql = string.Join(",", namesOfProps);
            var table = this._efCoreConnection.GetTableName<TEntity>();

            var sql = string.Format("UPDATE {0} SET {1} WHERE {2} = '{3}'", table, updatesSql, id, idValue);

            return sql;
        }

        public virtual void UpdateByProp(string value, object updates, string prop)
        {
            var query = this.BuildUpdateQuery(updates, value, prop);

            var rawsAffected = this._efCoreConnection.Database.ExecuteSqlRaw(query);

            if (rawsAffected != 0)
            {
                var entity = this._dbSet.Find(value)!;
                this._efCoreConnection.Entry(entity).Reload();
            }

        }
        #endregion

        #region Exist
        public virtual async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await this._dbSet.AnyAsync(predicate).ConfigureAwait(false);
        }

        public bool Exist(Expression<Func<TEntity, bool>> predicate)
        {
            return this._dbSet.Any(predicate);
        }
        #endregion

        #region Unity
        public virtual async Task CreateWithoutCommitAsync(TEntity entity)
        {
            await this._dbSet.AddAsync(entity).ConfigureAwait(false);
        }

        public virtual void CreateWithoutCommit(TEntity entity)
        {
            this._dbSet.Add(entity);
        }

        #endregion
    }
}