using CQ.UnitOfWork.Entities.DataAccessConfig;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Entities.Context
{
    public abstract class EfCoreContext : DbContext
    {
        private readonly EfCoreConfig? _config;

        private readonly string _configToUse;

        public EfCoreContext(EfCoreConfig config)
        {
            this._config = config;
            this._configToUse = "efCoreConfig";
        }

        public EfCoreContext(DbContextOptions optionsBuilder) : base(optionsBuilder)
        {
            this._configToUse = "dbContextOptions";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (this._configToUse == "dbContextOptions")
            {
                return;
            }

            this.Assert();

            if (EfCoreDataBaseEngines.SQL == this._config.Engine)
            {
                optionsBuilder.UseSqlServer(this._config.DatabaseConnection.ConnectionString);
            }

            if(EfCoreDataBaseEngines.SQL_LITE == this._config.Engine)
            {
                optionsBuilder.UseSqlite(new SqliteConnection(this._config.DatabaseConnection.ConnectionString));
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var contextAssembly = Assembly.GetExecutingAssembly();
            modelBuilder.ApplyConfigurationsFromAssembly(contextAssembly);
        }

        private void Assert()
        {
            if (this._config is null)
            {
                throw new ArgumentNullException("config");
            }

            this._config.Assert();
        }

        public void OpenConnection()
        {
            this.Database.OpenConnection();
        }

        public void EnsureCreated()
        {
            this.Database.EnsureCreated();
        }

        public void EnsureDeleted()
        {
            this.Database.EnsureDeleted();
        }
    }
}
