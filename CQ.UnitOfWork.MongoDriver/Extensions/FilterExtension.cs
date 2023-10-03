using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.MongoDriver.Extensions
{
    internal static class FilterExtension
    {
        public static FilterDefinition<TEntity> NullableWhere<TEntity>(this FilterDefinitionBuilder<TEntity> filter, Expression<Func<TEntity, bool>>? predicate)
        {
            return predicate is null ? filter.Where(e => true) : filter.Where(predicate);
        }
    }
}
