using CQ.UnitOfWork.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CQ.UnitOfWork
{
    internal class UnitOfWorkService : IUnitOfWork
    {
        private readonly IServiceProvider _services;

        private IDatabaseContext _unitContext;

        public UnitOfWorkService(IServiceProvider services)
        {
            this._services = services;
        }

        public IRepository<TEntity> GetEntityRepository<TEntity>() where TEntity : class
        {
            var entityRepository= this._services.GetRequiredService<IRepository<TEntity>>();

            return entityRepository;
        }

        public IRepository<TEntity> GetUnitRepository<TEntity, TContext>()
            where TEntity : class
            where TContext : IDatabaseContext
        {
            var context = this._services.GetRequiredService<TContext>();

            this._unitContext = context;

            var repository = this._services.GetRequiredService<IUnitRepository>();

            repository.SetContext(context);

            return repository;
        }

        public async Task CommitChangesAsync()
        {
            if(this._unitContext == null) { throw new InvalidOperationException($"Unit context not setted"); }

            await this._unitContext.SaveChangesAsync().ConfigureAwait(false);
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