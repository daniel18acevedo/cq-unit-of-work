using CQ.UnitOfWork.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Core
{
    public class EfCoreRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet;

        public EfCoreRepository(EfCoreContext efCoreContext)
        {
            this._dbSet = efCoreContext.GetEntitySet<TEntity>();
        }

        public Task<TEntity> CreateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetByPropAsync(string value, string? prop = null)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
