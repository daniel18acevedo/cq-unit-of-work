using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Abstractions.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CQ.UnitOfWork
{
    public static class UnitOfWorkInit
    {
        public static void AddUnitOfWork(this IServiceCollection services, LifeCycle lifeCycle=LifeCycle.SCOPED)
        {
            services.AddService<IUnitOfWork, UnitOfWorkService>(lifeCycle);
        }
    }
}
