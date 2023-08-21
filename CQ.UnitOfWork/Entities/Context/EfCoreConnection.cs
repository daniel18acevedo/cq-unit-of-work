using CQ.UnitOfWork.Entities.Context;
using CQ.UnitOfWork.Entities.DataAccessConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities
{
    public class EfCoreConnection : IDatabaseConnection
    {
        private readonly DbContext _dbContext;

        public EfCoreConnection(DbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public bool Ping()
        {
            var ping = this._dbContext.Database.ExecuteSqlRaw("SELECT 1 FROM USERS;");

            return ping == 1;
        }

        public DbSet<TEntity> GetEntitySet<TEntity>()
            where TEntity : class
        {
            return this._dbContext.Set<TEntity>();
        }

        public string GetTableName<TEntity>()
        {
            return $"{typeof(TEntity).Name}s";
        }

        public async Task SaveChangesAsync()
        {
            await this._dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public void SaveChanges() 
        {
            this._dbContext.SaveChanges();
        }
    }
}
