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
    public class EfCoreContext : DatabaseContext, IDataBaseContext
    {
        private readonly DbContext _dbContext;

        public EfCoreContext(DbContext dbContext):base(Orms.EF_CORE)
        {
            this._dbContext = dbContext;
        }

        public bool Ping()
        {
            var ping = this._dbContext.Database.ExecuteSqlRaw("SELECT 1;");

            return ping == 1;
        }

        public DbSet<TEntity> GetEntitySet<TEntity>()
            where TEntity : class
        {
            return this._dbContext.Set<TEntity>();
        }
    }
}
