using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Abstractions.Extensions;
using CQ.UnitOfWork.EfCore.Abstractions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.EfCore
{
    public static class EfCoreInit
    {
        public static void AddEfCore<TContext>(this IServiceCollection services, EfCoreConfig config, LifeCycle lifeCycle = LifeCycle.SCOPED) where TContext : EfCoreContext
        {
            config.Assert();

            var actions = (DbContextOptionsBuilder optionsBuilder) =>
            {
                switch (config.Engine)
                {
                    case EfCoreDataBaseEngines.SQL:
                        {
                            optionsBuilder.UseSqlServer(config.DatabaseConnection.ConnectionString);
                            break;
                        }

                    case EfCoreDataBaseEngines.SQL_LITE:
                        {
                            optionsBuilder.UseSqlite(new SqliteConnection(config.DatabaseConnection.ConnectionString));
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException($"Engine {config.Engine} not supported");
                        }
                }

                if (config.UseDefaultQueryLogger)
                {
                    optionsBuilder.LogTo(Console.WriteLine);
                }
                else if (config.Logger is not null)
                {
                    optionsBuilder.LogTo(config.Logger);
                }
            };
            var lifeTime = lifeCycle == LifeCycle.SCOPED ? ServiceLifetime.Scoped : lifeCycle == LifeCycle.TRANSIENT ? ServiceLifetime.Transient : ServiceLifetime.Singleton;

            services.AddDbContext<EfCoreContext, TContext>(actions, lifeTime);
        }

        public static void AddEfCoreRepository<TEntity>(this IServiceCollection services, LifeCycle lifeCycle=LifeCycle.SCOPED) where TEntity : class
        {
            services.AddService<Repository<TEntity>, EfCoreRepository<TEntity>>(lifeCycle);
            services.AddService<IRepository<TEntity>, EfCoreRepository<TEntity>>(lifeCycle);
            services.AddService<IEfCoreRepository<TEntity>, EfCoreRepository<TEntity>>(lifeCycle);
        }
    }
}
