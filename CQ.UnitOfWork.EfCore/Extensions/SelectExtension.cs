using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.EfCore.Extensions
{
    internal static class SelectExtension
    {
        public static IQueryable<TResult> SelectTo<TEntity, TResult>(this IQueryable<TEntity> source)
        {
            var elementType = typeof(TEntity);

            // input parameter "o"
            var parameter = Expression.Parameter(elementType, "o");

            var resultType = typeof(TResult);
            // new statement "new Data()"
            var elementCreated = Expression.New(resultType);


            var properties = resultType.GetProperties().Select(p => p.Name).ToList();
            // create initializers
            var bindings = properties.Where(property =>
            {
                bool exist = false;
                try
                {
                    elementType.GetProperty(property);

                    exist = true;
                }
                catch (ArgumentNullException) { }

                return exist;
            }
            ).Select(property =>
            {
                // property "Field1"
                var originalProperty = elementType.GetProperty(property);

                // original value "o.Field1"
                var callingProperty = Expression.Property(parameter, originalProperty);

                // property "Field1"
                var propertyToSet = resultType.GetProperty(property);

                // set value "Field1 = o.Field1"
                return Expression.Bind(propertyToSet, callingProperty);
            }
            );

            IQueryable<TResult> elementsToReturn = null;

            if (bindings.Any())
            {
                // initialization "new Data { Field1 = o.Field1, Field2 = o.Field2 }"
                var elementInit = Expression.MemberInit(elementCreated, bindings);

                // expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
                var lambda = Expression.Lambda<Func<TEntity, TResult>>(elementInit, parameter);

                // compile to Func<Data, Data>
                elementsToReturn = source.Select(lambda);
            }

            return elementsToReturn;
        }
    }
}
