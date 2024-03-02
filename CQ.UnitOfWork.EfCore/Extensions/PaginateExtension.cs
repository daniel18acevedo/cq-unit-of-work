using System;
using System.Linq;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Extensions
{
    public static class PaginateExtension
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> elements, int page = 1, int pageSize = 10)
        {
            if(page <= 0 || pageSize <= 0)
            {
                return elements;
            }

            var skipTo = (page - 1) * pageSize;

            return elements.Skip(skipTo).Take(pageSize);
        }
    }
}
