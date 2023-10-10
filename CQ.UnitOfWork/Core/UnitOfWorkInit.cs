using CQ.UnitOfWork.Abstractions;
using CQ.ServiceExtension;
using Microsoft.Extensions.DependencyInjection;

namespace CQ.UnitOfWork
{
    public static class UnitOfWorkInit
    {
        public static void AddUnitOfWork(this IServiceCollection services, LifeTime lifeCycle = LifeTime.Scoped)
        {
            services.AddService<IUnitOfWork, UnitOfWorkService>(lifeCycle);
        }
    }
}
