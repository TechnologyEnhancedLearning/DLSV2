namespace DigitalLearningSolutions.Data.Helpers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using Microsoft.AspNetCore.Http;

    public static class FilteringHelper
    {
        public const char Separator = '|';
        public const char FilterSeparator = '╡';
        public const string EmptyValue = "╳";
        public const string EmptyFiltersCookieValue = "CLEAR";
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
            bool clearFilters,
            HttpRequest request,
            string cookieName,
            string? defaultFilterValue = null
        )
        {
            if (existingFilterString == null && newFilterToAdd == null && !clearFilters)
            {
                return request.Cookies.ContainsKey(cookieName)
                    ? request.Cookies[cookieName] == EmptyFiltersCookieValue ? null : request.Cookies[cookieName]
                    : defaultFilterValue;
            }

            if (clearFilters)
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

        public static (IEnumerable<T> filteredItems, string? appliedFilterString) FilterOrResetFilterToDefault<T>(
            IEnumerable<T> items,
            FilterOptions filterOptions
        )
            where T : BaseSearchableItem
        {
            if (AvailableFiltersContainsAllSelectedFilters(filterOptions))
            {
                return (FilterItems(items.AsQueryable(), filterOptions.FilterString),
                    filterOptions.FilterString);
            }

            return filterOptions.DefaultFilterString != null
                ? (FilterItems(items.AsQueryable(), filterOptions.DefaultFilterString),
                    filterOptions.DefaultFilterString)
                : (items, null);
        }

        private static bool AvailableFiltersContainsAllSelectedFilters(FilterOptions filterOptions)
        {
            var currentFilters = filterOptions.FilterString?.Split(FilterSeparator).ToList() ??
                                 new List<string>();

            return currentFilters.All(filter => AvailableFiltersContainsFilter(filterOptions.AvailableFilters, filter));
        }

        private static bool AvailableFiltersContainsFilter(IEnumerable<FilterModel> availableFilters, string filter)
        {
            return availableFilters.Any(filterModel => FilterOptionsContainsFilter(filter, filterModel.FilterOptions));
        }

        private static bool FilterOptionsContainsFilter(
            string filter,
            IEnumerable<FilterOptionModel> filterOptions
        )
        {
            return filterOptions.Any(filterOption => filterOption.FilterValue == filter);
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
                // Course Admin Fields and Centre Registration Prompt filters rely on properties of the models
                // called Answer1, Answer2 etc. We append the prompt text in brackets to the property
                // name when setting up these filters so that we can check whether they are valid filters for another course etc.
                // e.g. Answer1(Access Permission)|Answer1(Access Permissions)|FREETEXTBLANKVALUE would not be a valid filter for
                // a course with admin field "Priority Access" at position 1, or a course with no admin field at position 1,
                // but would technically be able to be used for filtering the models.
                // We have to strip the bracketed value here so we return back to just the property name string
                PropertyName = splitFilter[1].Split('(')[0];
                PropertyValue = splitFilter[2];
            }

            public string Group { get; }

            public string PropertyName { get; }

            public string PropertyValue { get; }
        }
    }
}
