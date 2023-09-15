using CQ.UnitOfWork.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CQ.UnitOfWork
{
    internal class UnitOfWorkService : IUnitOfWork
    {
        private readonly IServiceProvider _services;

        public UnitOfWorkService(IServiceProvider services)
        {
            this._services = services;
        }

        public IRepository<TEntity> GetEntityRepository<TEntity>() where TEntity : class
        {
            var entityRepositories= this._services.GetServices<IRepository<TEntity>>();

            if(entityRepositories is null || !entityRepositories.Any())
            {
                throw new ArgumentException($"Repository for entity ${typeof(TEntity).Name} not loaded");
            }

            var entityRepository = entityRepositories.FirstOrDefault();

            if(entityRepository is null)
            {
                throw new ArgumentException($"Repository for entity ${typeof(TEntity).Name} not loaded");
            }

            return entityRepository;
        }

        public TRepository GetRepository<TRepository>() where TRepository : class
        {
            var repository = this._services.GetService<TRepository>();

            if(repository is null)
            {
                throw new ArgumentException($"Repository {typeof(TRepository).Name} not loaded");
            }

            return repository;
        }
    }
}