using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tablyx.CommonUtils.DataAccess.Extensions
{
    public static class NullableOrderByExtension
    {
        public static IQueryable<T> NullableOrderBy<T>(this IQueryable<T> elements, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        {
            return orderBy is null ? elements : orderBy(elements);
        }
    }
}
