using CQ.UnitOfWork.Core;
using CQ.UnitOfWork.Entities;
using CQ.UnitOfWork.Entities.DataAccessConfig;
using CQ.UnitOfWork.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Init
{
    public static class UnitOfWorkInit
    {
        public static void AddUnitOfWorkWithMongo(this IServiceCollection services, LifeCycles unitOfWorkLifeCycle, MongoConfig mongoConfig)
        {
            services.AddMongoDriverOrm(mongoConfig);

            services.AddService<IUnitOfWork, UnitOfWorkService>(unitOfWorkLifeCycle);
        }

        public static void AddUnitOfWorkWithEfCore(this IServiceCollection services, LifeCycles unitOfWorkLifeCycle, EfCoreConfig efCoreConfig)
        {
            services.AddEfCoreOrm(efCoreConfig);

            services.AddService<IUnitOfWork, UnitOfWorkService>(unitOfWorkLifeCycle);
        }

        public static void AddUnitOfWork(this IServiceCollection services, LifeCycles unitOfWorkLifeCycle)
        {
            services.AddService<IUnitOfWork, UnitOfWorkService>(unitOfWorkLifeCycle);
        }

        internal static void AddService<TImplementation>(this IServiceCollection services, LifeCycles lifeCycle, Func<IServiceProvider, TImplementation> implementationFactory)
            where TImplementation : class
        {
            switch (lifeCycle)
            {
                case LifeCycles.TRANSIENT:
                    {
                        services.AddTransient(implementationFactory);
                        break;
                    }
                case LifeCycles.SCOPED:
                    {
                        services.AddScoped(implementationFactory);
                        break;
                    }
                case LifeCycles.SINGLETON:
                    {
                        services.AddSingleton(implementationFactory);
                        break;
                    }
            }
        }

        internal static void AddService<TService, TImplementation>(this IServiceCollection services, LifeCycles lifeCycle)
            where TService : class
            where TImplementation : class, TService
        {
            switch (lifeCycle)
            {
                case LifeCycles.TRANSIENT:
                    {
                        services.AddTransient<TService, TImplementation>();
                        break;
                    }
                case LifeCycles.SCOPED:
                    {
                        services.AddScoped<TService, TImplementation>();
                        break;
                    }
                case LifeCycles.SINGLETON:
                    {
                        services.AddSingleton<TService, TImplementation>();
                        break;
                    }
            }
        }

        internal static void AddService<TService>(this IServiceCollection services, LifeCycles lifeCycle)
            where TService : class
        {
            switch (lifeCycle)
            {
                case LifeCycles.TRANSIENT:
                    {
                        services.AddTransient<TService>();
                        break;
                    }
                case LifeCycles.SCOPED:
                    {
                        services.AddScoped<TService>();
                        break;
                    }
                case LifeCycles.SINGLETON:
                    {
                        services.AddSingleton<TService>();
                        break;
                    }
            }
        }

        internal static void AddService<TService>(this IServiceCollection services, LifeCycles lifeCycle, TService value)
            where TService : class
        {
            services.AddService(lifeCycle, (serviceProvider) => value);
        }
    }
}
