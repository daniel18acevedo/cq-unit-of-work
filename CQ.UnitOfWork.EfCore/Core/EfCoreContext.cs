using CQ.UnitOfWork.Abstractions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.EfCore
{
    public abstract class EfCoreContext : DbContext, IDatabaseContext
    {
        /// <summary>
        /// Necessary when using AddDbContext
        /// </summary>
        /// <param name="options"></param>
        public EfCoreContext(DbContextOptions options) : base(options)
        { 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var contextAssembly = Assembly.GetExecutingAssembly();
            modelBuilder.ApplyConfigurationsFromAssembly(contextAssembly);
        }

        public virtual void OpenConnection()
        {
            this.Database.OpenConnection();
        }

        public virtual void EnsureCreated()
        {
            this.Database.EnsureCreated();
        }

        public virtual void EnsureDeleted()
        {
            this.Database.EnsureDeleted();
        }

        public virtual bool Ping(string? collection = null)
        {
            try
            {
                return this.Database.CanConnect();
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error pinging the database: {ex.Message}");
                return false;
            }
        }

        public virtual DbSet<TEntity> GetEntitySet<TEntity>()
            where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public virtual string GetTableName<TEntity>()
        {
            var model = this.Model.FindEntityType(typeof(TEntity));

            var tableName = model?.GetTableName();

            if (string.IsNullOrEmpty(tableName))
            {
                return string.Empty;
            }

            return tableName;
        }

        public virtual async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual DatabaseInfo GetDatabaseInfo()
        {
            var databaseInfo = new DatabaseInfo
            {
                Provider = this.Database.ProviderName,
                Name = this.Database.GetDbConnection().Database
            };
            var dbConnection = this.Database.GetDbConnection();

            var connectionString = dbConnection.ConnectionString;

            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString,
            };

            if (builder.TryGetValue("Database", out var dbName))
            {
                databaseInfo.Name = dbName.ToString();

                return databaseInfo;
            }

            return databaseInfo;
        }
    }
}
