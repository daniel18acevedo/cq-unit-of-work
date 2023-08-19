using CQ.UnitOfWork.Entities.DataAccessConfig;
using CQ.UnitOfWork.Entities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CQ.UnitOfWork.Core;
using CQ.UnitOfWork.Exceptions;
using CQ.UnitOfWork.Entities.Context;

namespace CQ.UnitOfWork.Init
{
    public static class EfCoreOrmInit
    {
        public static void AddEfCoreOrm(this IServiceCollection services, EfCoreConfig efCoreConfig)
        {
            var efCoreDatabase = new EfCoreConnection(efCoreConfig);

            services.AddService<DbContext>(efCoreConfig.LifeCycle, efCoreDatabase);

            services.AddService<IDataBaseContext, EfCoreContext>(efCoreConfig.LifeCycle);
            
            services.AddService<EfCoreContext>(efCoreConfig.LifeCycle);
        }

        public static void AddEfCoreContext<TContext>(this IServiceCollection services, LifeCycles lifeCycle, TContext context)
            where TContext : EfCoreConnection
        {
            services.AddService(lifeCycle, context);
            services.AddService<OrmConfig>(LifeCycles.SINGLETON, new EfCoreConfig
            {
                LifeCycle = lifeCycle,
            });
        }

        public static void AddEfCoreRepository<T>(this IServiceCollection services, LifeCycles lifeCycle) where T : class
        {
            services.AddService(lifeCycle, (serviceProvider) =>
            {
                var efCoreDatabase = serviceProvider.GetService<EfCoreContext>();

                if (efCoreDatabase is null)
                {
                    throw new OrmNotSupportedException(Orms.EF_CORE);
                }

                return new EfCoreRepository<T>(efCoreDatabase);
            });
        }
    }
}
