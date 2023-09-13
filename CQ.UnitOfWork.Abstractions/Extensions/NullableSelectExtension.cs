using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions.Extensions
{
    public static class NullableSelectExtension
    {
        public static IQueryable<T> NullableSelect<T>(this IQueryable<T> elements, Expression<Func<T, T>> select)
        {
            return select is null ? elements : elements.Select(select);
        }
    }
}