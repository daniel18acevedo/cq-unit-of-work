using CQ.UnitOfWork.Abstractions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.EfCore
{
    public abstract class EfCoreContext : DbContext, IDatabaseContext
    {
        private readonly EfCoreConfig? _config;

        public EfCoreContext() { }

        public EfCoreContext(EfCoreConfig config)
        {
            this._config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured || this._config is null)
            {
                return;
            }

            this._config.Assert();
            switch (this._config.Engine)
            {
                case EfCoreDataBaseEngine.SQL:
                    {
                        optionsBuilder.UseSqlServer(this._config.DatabaseConnection.ConnectionString);
                        break;
                    }

                case EfCoreDataBaseEngine.SQL_LITE:
                    {
                        optionsBuilder.UseSqlite(new SqliteConnection(this._config.DatabaseConnection.ConnectionString));
                        break;
                    }

                default:
                    {
                        throw new ArgumentException($"Engine {this._config.Engine} not supported");
                    }
            }

            if (this._config.UseDefaultQueryLogger)
            {
                optionsBuilder.LogTo(Console.WriteLine);
            }
            else if (this._config.Logger is not null)
            {
                optionsBuilder.LogTo(this._config.Logger);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var contextAssembly = Assembly.GetExecutingAssembly();
            modelBuilder.ApplyConfigurationsFromAssembly(contextAssembly);
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

        public bool Ping(string? collection = null)
        {
            var ping = this.Database.ExecuteSqlRaw($"SELECT 1 FROM {collection ?? "USERS"};");

            return ping == 1;
        }

        public DbSet<TEntity> GetEntitySet<TEntity>()
            where TEntity : class
        {
            return this.Set<TEntity>();
        }

        public string GetTableName<TEntity>()
        {
            return $"{typeof(TEntity).Name}s";
        }

        public async Task SaveChangesAsync()
        {
            await this.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
