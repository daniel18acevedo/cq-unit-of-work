using CQ.UnitOfWork.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CQ.UnitOfWork
{
    internal class UnitOfWorkService : IUnitOfWork
    {
        private readonly IServiceProvider _services;

        private readonly OrmConfig _defaultOrmConfig;

        public UnitOfWorkService(IServiceProvider services, OrmConfig defaultOrmConfig)
        {
            this._services = services;
            this._defaultOrmConfig = defaultOrmConfig;
        }

        public IRepository<TEntity> GetEntityRepository<TEntity>(Orm? orm, string? databaseName) where TEntity : class
        {
            orm ??= this._defaultOrmConfig.Orm;
            databaseName ??= this._defaultOrmConfig.DatabaseConnection.DatabaseName;

            var entityRepositories= this._services.GetServices<Repository<TEntity>>();

            if(entityRepositories is null || !entityRepositories.Any())
            {
                throw new ArgumentException($"Repository for entity ${typeof(TEntity).Name} not loaded");
            }

            entityRepositories = entityRepositories.Where(repo => repo.Orm == orm);

            if(!entityRepositories.Any())
            {
                throw new ArgumentException($"Repository for entity ${typeof(TEntity).Name} of orm {orm} not loaded");
            }

            var entityRepository = entityRepositories.FirstOrDefault(repo => repo.ConnectedTo == databaseName);

            if(entityRepository is null)
            {
                throw new ArgumentException($"Repository for entity ${typeof(TEntity).Name} of orm {orm} connected to {databaseName} not loaded");
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