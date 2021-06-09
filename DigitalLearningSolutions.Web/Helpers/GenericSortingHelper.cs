namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public static class GenericSortingHelper
    {
        /// <summary>
        /// Sorts a list of items by property name or names.
        /// </summary>
        /// <typeparam name="T">Type which implements BaseSearchableItem</typeparam>
        /// <param name="items">The items to be sorted</param>
        /// <param name="sortBy">Ordered comma separated list of property names to sort by</param>
        /// <param name="sortDirection">Direction to sort</param>
        /// <returns>Sorted list of items</returns>
        public static IEnumerable<T> SortAllItems<T>(
            IQueryable<T> items,
            string sortBy,
            string sortDirection
        ) where T : BaseSearchableItem
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return items;
            }

            var sortByArray = sortBy.Split(',');

            var result = sortDirection == BaseSearchablePageViewModel.DescendingText
                ? items.OrderByDescending(sortByArray[0])
                : items.OrderBy(sortByArray[0]);

            if (sortByArray.Length > 1)
            {
                for (var i = 1; i < sortByArray.Length; i++)
                {
                    result = sortDirection == BaseSearchablePageViewModel.DescendingText
                        ? result.ThenByDescending(sortByArray[1])
                        : result.ThenBy(sortByArray[1]);
                }
            }

            return result;
        }
    }
}
