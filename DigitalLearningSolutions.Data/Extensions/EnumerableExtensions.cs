namespace DigitalLearningSolutions.Data.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : class
        {
            return enumerable.Where(e => e != null).Select(e => e!);
        }
    }
}
