using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.MongoDriver.Extensions
{
    internal static class FindExtension
    {
        public static IFindFluent<TEntity, TEntity> NullableFind<TEntity>(this IMongoCollection<TEntity> collection, Expression<Func<TEntity, bool>> predicate)
        {
            return predicate is null ? collection.Find(e => true) : collection.Find(predicate);
        }
    }
}
