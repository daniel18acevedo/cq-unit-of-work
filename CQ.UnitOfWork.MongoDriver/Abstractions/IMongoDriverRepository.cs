using CQ.UnitOfWork.Abstractions;

namespace CQ.UnitOfWork.MongoDriver.Abstractions
{
    public interface IMongoDriverRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
    }
}