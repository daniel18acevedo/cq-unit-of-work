﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.Extensions
{
    public static class NullableWhereExtension
    {
        public static IQueryable<T> NullableWhere<T>(this IQueryable<T> elements, Expression<Func<T, bool>> condition)
        {
            return condition is null ? elements : elements.Where(condition);
        }
    }
}
