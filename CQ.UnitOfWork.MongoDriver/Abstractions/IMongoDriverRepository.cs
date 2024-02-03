using CQ.UnitOfWork.Abstractions;

namespace CQ.UnitOfWork.MongoDriver.Abstractions
{
    public interface IMongoDriverRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        Task UpdateByPropAsync(string value, object updates, string? prop = null);

        void UpdateByProp(string value, object updates, string? prop = null);
    }
}