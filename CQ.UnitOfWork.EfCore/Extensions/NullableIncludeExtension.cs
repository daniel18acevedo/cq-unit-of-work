using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CQ.UnitOfWork.EfCore.Extensions
{
    public static class NullableIncludeExtension
    {
        public static IQueryable<T> NullableInclude<T>(this IQueryable<T> elements, Func<IQueryable<T>, IIncludableQueryable<T, object>> include)
        {
            return include is null ? elements : include(elements);
        }
    }
}
