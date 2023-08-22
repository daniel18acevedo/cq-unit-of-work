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
        public static void AddUnitOfWorkWithMongo(this IServiceCollection services, OrmServiceConfig<MongoConfig> ormConfig, LifeCycles unitOfWorkLifeCycle = LifeCycles.SCOPED)
        {
            services.AddMongoDriverOrm(ormConfig.Config, ormConfig.LifeCycle);

            services.AddService<IUnitOfWork, UnitOfWorkService>(unitOfWorkLifeCycle);
        }

        public static void AddUnitOfWorkWithEfCore<TContext>(this IServiceCollection services, OrmServiceConfig<EfCoreConfig> ormConfig, LifeCycles unitOfWorkLifeCycle = LifeCycles.SCOPED)
            where TContext : EfCoreContext
        {
            services.AddEfCoreOrm<TContext>(ormConfig.Config, ormConfig.LifeCycle);

            services.AddService<IUnitOfWork, UnitOfWorkService>(unitOfWorkLifeCycle);
        }

        public static void AddUnitOfWork(this IServiceCollection services, LifeCycles unitOfWorkLifeCycle = LifeCycles.SCOPED)
        {
            services.AddService<IUnitOfWork, UnitOfWorkService>(unitOfWorkLifeCycle);
        }

        internal static void AddService<TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory, LifeCycles lifeCycle = LifeCycles.SCOPED)
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

        internal static void AddService<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory, LifeCycles lifeCycle = LifeCycles.SCOPED)
            where TService : class
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

        internal static void AddService<TService, TImplementation>(this IServiceCollection services, LifeCycles lifeCycle = LifeCycles.SCOPED)
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

        internal static void AddService<TService>(this IServiceCollection services, LifeCycles lifeCycle = LifeCycles.SCOPED)
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

        internal static void AddService<TService>(this IServiceCollection services, TService value, LifeCycles lifeCycle = LifeCycles.SCOPED)
            where TService : class
        {
            services.AddService((serviceProvider) => value, lifeCycle);
        }

        internal static void AddService<TService, TImplementation>(this IServiceCollection services, TImplementation value, LifeCycles lifeCycle = LifeCycles.SCOPED)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddService<TService, TImplementation>((serviceProvider) => value, lifeCycle);
        }
    }
}
