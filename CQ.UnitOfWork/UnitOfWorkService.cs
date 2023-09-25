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

        /// <summary>
        /// Gets a proper repository to handle entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IRepository<TEntity> GetEntityRepository<TEntity>() where TEntity : class
        {
            var entityRepository = this._services.GetRequiredService<IRepository<TEntity>>();

            return entityRepository;
        }

        /// <summary>
        /// Usefull when have multiple context
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public IRepository<TEntity> GetUnitRepository<TEntity, TContext>()
            where TEntity : class
            where TContext : IDatabaseContext
        {

            if (this._unitContext is null)
            {
                var context = this._services.GetRequiredService<TContext>();

                this._unitContext = context;
            }

            var repository = this._services.GetRequiredService<IUnitRepository<TEntity>>();

            repository.SetContext(this._unitContext);

            return repository;
        }

        /// <summary>
        /// Usefull when have only one context and want repositoy to use same context
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IRepository<TEntity> GetUnitRepository<TEntity>()
            where TEntity : class
        {
            if (this._unitContext is null)
            {
                var context = this._services.GetRequiredService<IDatabaseContext>();

                this._unitContext = context;
            }

            var repository = this._services.GetRequiredService<IUnitRepository<TEntity>>();

            repository.SetContext(this._unitContext);

            return repository;
        }

        /// <summary>
        /// Commit several changes at once. Useful for unit repositories
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task CommitChangesAsync()
        {
            if (this._unitContext == null) { throw new InvalidOperationException($"Unit context not setted"); }

            await this._unitContext.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets specific repository. If not defined, ArgumentException is thrown
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public TRepository GetRepository<TRepository>() where TRepository : class
        {
            var repository = this._services.GetService<TRepository>();

            if (repository is null)
            {
                throw new ArgumentException($"Repository {typeof(TRepository).Name} not loaded");
            }

            return repository;
        }
    }
}