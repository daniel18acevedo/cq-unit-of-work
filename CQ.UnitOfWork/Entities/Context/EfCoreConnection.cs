using CQ.UnitOfWork.Entities.DataAccessConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities.Context
{
    public class EfCoreConnection : DbContext
    {
        private readonly EfCoreConfig _config;

        public EfCoreConnection() { }

        public EfCoreConnection(EfCoreConfig config)
        {
            this._config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (EfCoreDataBaseEngines.SQL == this._config.Engine)
            {
                optionsBuilder.UseSqlServer(this._config.DataBaseConnection.ConnectionString);
            }

            if (this._config.EnabledDefaultQueryLogger)
            {
                optionsBuilder.LogTo(Console.WriteLine);
            }

            if (this._config.Logger is not null)
            {
                optionsBuilder.LogTo(this._config.Logger);
            }
        }
    }
}
