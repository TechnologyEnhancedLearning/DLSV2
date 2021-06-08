namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public static class GenericSortingHelper
    {
        public static IEnumerable<T> SortAllItems<T>(
            IEnumerable<T> items,
            string sortBy,
            string sortDirection
        )
        {
            return sortDirection == BaseSearchablePageViewModel.DescendingText
                ? items.OrderByDescending(sortBy)
                : items.OrderBy(sortBy);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IEnumerable<T> source, string propertyName)
        {
            return source.ApplyOrdering(propertyName, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IEnumerable<T> source, string propertyName)
        {
            return source.ApplyOrdering(propertyName, "OrderByDescending");
        }

        private static IOrderedQueryable<T> ApplyOrdering<T>(
            this IEnumerable<T> source,
            string propertyName,
            string methodName
        )
        {
            var type = typeof(T);
            var propertyInfo = type.GetProperty(propertyName)!;

            var arg = Expression.Parameter(type);
            Expression expr = arg;
            expr = Expression.Property(expr, propertyInfo);
            type = propertyInfo.PropertyType;

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            var result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                              && method.IsGenericMethodDefinition
                              && method.GetGenericArguments().Length == 2
                              && method.GetParameters().Length == 2
                )
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source.AsQueryable(), lambda })!;

            return (IOrderedQueryable<T>)result;
        }
    }
}
