namespace CQ.UnitOfWork.Abstractions
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> GetEntityRepository<TEntity>(Orm? orm) where TEntity : class;

        TRepository GetRepository<TRepository>() where TRepository : class;
    }
}