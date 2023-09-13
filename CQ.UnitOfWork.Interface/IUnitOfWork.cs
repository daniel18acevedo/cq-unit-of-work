namespace CQ.UnitOfWork.Interface
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> GetEntityRepository<TEntity>(Orm? orm) where TEntity : class;

        TRepository GetRepository<TRepository>() where TRepository : class;

        //IEfCoreRepository<TEntity> GetEfCoreRepository<TEntity>() where TEntity : class;

        //IMongoRepository<TEntity> GetMongoRepository<TEntity>(string? collectionName = null) where TEntity : class;
    }
}