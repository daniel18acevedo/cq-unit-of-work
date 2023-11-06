using CQ.UnitOfWork.Abstractions;
using CQ.ServiceExtension;
using Microsoft.Extensions.DependencyInjection;

namespace CQ.UnitOfWork
{
    public static class UnitOfWorkDependencyInjection
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services, LifeTime lifeCycle = LifeTime.Scoped)
        {
            services.AddService<IUnitOfWork, UnitOfWorkService>(lifeCycle);
            
            return services;
        }
    }
}
