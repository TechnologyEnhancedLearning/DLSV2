namespace DigitalLearningSolutions.Data.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public static class QueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return source.ThenBy(ToLambda<T>(propertyName));
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return source.ThenByDescending(ToLambda<T>(propertyName));
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string propertyName, object? propertyValue)
        {
            return source.Where(ToEqualityLambda<T>(propertyName, propertyValue));
        }

        public static IQueryable<T> WhereNullOrEmpty<T>(this IQueryable<T> source, string propertyName)
        {
            return source.Where(ToNullOrEmptyLambda<T>(propertyName));
        }

        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }

        private static Expression<Func<T, bool>> ToEqualityLambda<T>(string propertyName, object? comparisonValue)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var objectEquality = Expression.Equal(property, Expression.Constant(comparisonValue));

            return Expression.Lambda<Func<T, bool>>(objectEquality, parameter);
        }

        private static Expression<Func<T, bool>> ToNullOrEmptyLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var objectEquality = Expression.Or(
                Expression.Equal(property, Expression.Constant(null)),
                Expression.Equal(property, Expression.Constant(string.Empty))
            );

            return Expression.Lambda<Func<T, bool>>(objectEquality, parameter);
        }
    }
}
