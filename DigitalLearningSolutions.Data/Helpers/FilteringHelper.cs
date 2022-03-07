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
        public const string EmptyValue = "╳";
        public const string ClearString = "CLEAR";
        public const string FreeTextBlankValue = "FREETEXTBLANKVALUE";
        public const string FreeTextNotBlankValue = "FREETEXTNOTBLANKVALUE";

        public static string BuildFilterValueString(string group, string propertyName, string propertyValue)
        {
            return group + Separator + propertyName + Separator + propertyValue;
        }

        public static string? AddNewFilterToFilterString(string? existingFilterString, string? newFilterToAdd)
        {
            if (existingFilterString == null)
            {
                return newFilterToAdd;
            }

            if (newFilterToAdd == null || existingFilterString.Split(FilterSeparator).Contains(newFilterToAdd))
            {
                return existingFilterString;
            }

            return existingFilterString + FilterSeparator + newFilterToAdd;
        }

        public static string? GetFilterString(
            string? existingFilterString,
            string? newFilterToAdd,
            HttpRequest request,
            string cookieName,
            string? defaultFilterValue = null
        )
        {
            if (existingFilterString == null && newFilterToAdd == null)
            {
                return request.Cookies.ContainsKey(cookieName)
                    ? request.Cookies[cookieName]
                    : defaultFilterValue;
            }

            if (existingFilterString?.ToUpper() == ClearString)
            {
                existingFilterString = null;
            }

            return AddNewFilterToFilterString(existingFilterString, newFilterToAdd);
        }

        public static string? GetCategoryAndTopicFilterString(
            string? categoryFilterString,
            string? topicFilterString
        )
        {
            if (categoryFilterString == null && topicFilterString == null)
            {
                return null;
            }

            if (categoryFilterString == null)
            {
                return topicFilterString;
            }

            if (topicFilterString == null)
            {
                return categoryFilterString;
            }

            return topicFilterString + FilterSeparator + categoryFilterString;
        }

        public static IEnumerable<T> FilterItems<T>(
            IQueryable<T> items,
            string? filterString
        ) where T : BaseSearchableItem
        {
            var listOfFilters = filterString?.Split(FilterSeparator).ToList() ?? new List<string>();

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
            switch (propertyValue)
            {
                case EmptyValue:
                case FreeTextBlankValue:
                    return items.WhereNullOrEmpty(propertyName);
                case FreeTextNotBlankValue:
                    return items.Where(item => !items.WhereNullOrEmpty(propertyName).Contains(item));
                default:
                    return items.Where(propertyName, propertyValue);
            }
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
