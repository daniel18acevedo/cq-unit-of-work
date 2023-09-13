using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions.Extensions
{
    public static class AddServiceExtension
    {
        public static void AddService<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory, LifeCycle lifeCycle)
            where TService : class
            where TImplementation : class, TService
        {
            switch (lifeCycle)
            {
                case LifeCycle.SCOPED:
                    {
                        services.AddScoped<TService, TImplementation>(implementationFactory);
                        break;
                    }

                case LifeCycle.TRANSIENT:
                    {
                        services.AddTransient<TService, TImplementation>(implementationFactory);

                        break;
                    }
                case LifeCycle.SINGLETON:
                    {
                        services.AddSingleton<TService, TImplementation>(implementationFactory);
                        break;
                    }
            }
        }

        public static void AddService<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory, LifeCycle lifeCycle)
            where TService : class
        {
            switch (lifeCycle)
            {
                case LifeCycle.SCOPED:
                    {
                        services.AddScoped<TService>(implementationFactory);
                        break;
                    }

                case LifeCycle.TRANSIENT:
                    {
                        services.AddTransient<TService>(implementationFactory);

                        break;
                    }
                case LifeCycle.SINGLETON:
                    {
                        services.AddSingleton<TService>(implementationFactory);
                        break;
                    }
            }
        }

        public static void AddService<TService, TImplementation>(this IServiceCollection services, LifeCycle lifeCycle)
            where TService : class
            where TImplementation : class, TService
        {
            switch (lifeCycle)
            {
                case LifeCycle.SCOPED:
                    {
                        services.AddScoped<TService, TImplementation>();
                        break;
                    }

                case LifeCycle.TRANSIENT:
                    {
                        services.AddTransient<TService, TImplementation>();

                        break;
                    }
                case LifeCycle.SINGLETON:
                    {
                        services.AddSingleton<TService, TImplementation>();
                        break;
                    }
            }
        }
    }
}
