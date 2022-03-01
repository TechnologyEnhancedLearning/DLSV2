namespace DigitalLearningSolutions.Data.Helpers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.AspNetCore.Http;

    public static class FilteringHelper
    {
        public const char Separator = '|';
        public const char FilterSeparator = '╡';
        public const char EmptyValue = '╳';
        public const string ClearString = "CLEAR";

        public static string BuildFilterValueString(string group, string propertyName, string propertyValue)
        {
            return group + Separator + propertyName + Separator + propertyValue;
        }

        public static string? AddNewFilterToFilterBy(string? filterBy, string? newFilterValue)
        {
            if (filterBy == null)
            {
                return newFilterValue;
            }

            if (newFilterValue == null || filterBy.Split(FilterSeparator).Contains(newFilterValue))
            {
                return filterBy;
            }

            return filterBy + FilterSeparator + newFilterValue;
        }

        public static string? GetFilterBy(
            string? filterBy,
            string? filterValue,
            HttpRequest request,
            string cookieName,
            string? defaultFilterValue = null
        )
        {
            if (filterBy == null && filterValue == null)
            {
                return request.Cookies.ContainsKey(cookieName)
                    ? request.Cookies[cookieName]
                    : defaultFilterValue;
            }

            if (filterBy?.ToUpper() == ClearString)
            {
                filterBy = null;
            }

            return AddNewFilterToFilterBy(filterBy, filterValue);
        }

        public static string? GetCategoryAndTopicFilterBy(
            string? categoryFilterBy,
            string? topicFilterBy
        )
        {
            if (categoryFilterBy == null && topicFilterBy == null)
            {
                return null;
            }

            if (categoryFilterBy == null)
            {
                return topicFilterBy;
            }

            if (topicFilterBy == null)
            {
                return categoryFilterBy;
            }

            return topicFilterBy + FilterSeparator + categoryFilterBy;
        }

        public static IEnumerable<T> FilterItems<T>(
            IQueryable<T> items,
            string? filterBy
        ) where T : BaseSearchableItem
        {
            var listOfFilters = filterBy?.Split(FilterSeparator).ToList() ?? new List<string>();

            var appliedFilters = listOfFilters.Select(filter => new AppliedFilter(filter));

            foreach (var filterGroup in appliedFilters.GroupBy(a => a.Group))
            {
                var itemsToFilter = items;
                var setOfFilteredLists = filterGroup.Select(
                    af => FilterGroupItems(itemsToFilter, af.PropertyName, af.PropertyValue)
                );
                items = setOfFilteredLists.SelectMany(x => x).Distinct().AsQueryable();
            }

            return items;
        }

        private static IQueryable<T> FilterGroupItems<T>(
            IQueryable<T> items,
            string propertyName,
            string propertyValueString
        )
        {
            var propertyType = typeof(T).GetProperty(propertyName)!.PropertyType;
            var propertyValue = TypeDescriptor.GetConverter(propertyType).ConvertFromString(propertyValueString);
            return EmptyValue.ToString().Equals(propertyValue)
                ? items.WhereNullOrEmpty(propertyName)
                : items.Where(propertyName, propertyValue);
        }

        private class AppliedFilter
        {
            public AppliedFilter(string filterOption)
            {
                var splitFilter = filterOption.Split(Separator);
                Group = splitFilter[0];
                PropertyName = splitFilter[1];
                PropertyValue = splitFilter[2];
            }

            public string Group { get; }

            public string PropertyName { get; }

            public string PropertyValue { get; }
        }
    }
}
