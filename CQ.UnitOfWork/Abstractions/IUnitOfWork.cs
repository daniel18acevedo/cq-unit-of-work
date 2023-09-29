namespace CQ.UnitOfWork.Abstractions
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> GetEntityRepository<TEntity>() where TEntity : class;

        TRepository GetRepository<TRepository>() where TRepository : class;

        Task CommitChangesAsync();
    }
}