using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.EfCore.Extensions
{
    public static class NullableCountExtension
    {
        public static async Task<int> NullableCountAsync<T>(this IQueryable<T> elements, Expression<Func<T, bool>>? condition)
        {
            return condition == null ? await elements.CountAsync().ConfigureAwait(false) : await elements.CountAsync(condition).ConfigureAwait(false);
        }

        public static int NullableCount<T>(this IQueryable<T> elements, Expression<Func<T, bool>>? condition)
        {
            return condition == null ? elements.Count() : elements.Count(condition);
        }
    }
}
