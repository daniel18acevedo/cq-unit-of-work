using CQ.UnitOfWork.Core;
using CQ.UnitOfWork.Entities;
using CQ.UnitOfWork.Entities.Context;
using CQ.UnitOfWork.Entities.DataAccessConfig;
using CQ.UnitOfWork.Entities.ServiceConfig;
using Microsoft.Extensions.DependencyInjection;

namespace CQ.UnitOfWork.Init
{
    public static class UnitOfWorkInit
    {
        public static void AddUnitOfWorkWithMongo(this IServiceCollection services, LifeCycles unitOfWorkLifeCycle, OrmServiceConfig<MongoConfig> ormConfig)
        {
            services.AddMongoDriverOrm(ormConfig.LifeCycle, ormConfig.Config);

            services.AddService<IUnitOfWork, UnitOfWorkService>(unitOfWorkLifeCycle);
        }

        public static void AddUnitOfWorkWithEfCore<TContext>(this IServiceCollection services, LifeCycles unitOfWorkLifeCycle, OrmServiceConfig<TContext> ormConfig)
            where TContext : EfCoreContext
        {
            services.AddEfCoreOrm(ormConfig.LifeCycle, ormConfig.Config);

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

        internal static void AddService<TService, TImplementation>(this IServiceCollection services, LifeCycles lifeCycle, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService: class
            where TImplementation : class, TService
        {
            switch (lifeCycle)
            {
                case LifeCycles.TRANSIENT:
                    {
                        services.AddTransient<TService, TImplementation>(implementationFactory);
                        break;
                    }
                case LifeCycles.SCOPED:
                    {
                        services.AddScoped<TService, TImplementation>(implementationFactory);
                        break;
                    }
                case LifeCycles.SINGLETON:
                    {
                        services.AddSingleton<TService, TImplementation>(implementationFactory);
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
