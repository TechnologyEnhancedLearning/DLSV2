namespace DigitalLearningSolutions.Web.Extensions
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> key)
        {
            return enumerable.GroupBy(key).Select(g => g.First());
        }
    }
}
