﻿namespace DigitalLearningSolutions.Data.Helpers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
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
            return group + Separator + propertyName + Separator + propertyValue.Trim();
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
            var filterString = existingFilterString + FilterSeparator + newFilterToAdd;
            return RemoveDuplicateFilters(filterString, newFilterToAdd);
        }

        public static string? GetFilterString(
            string? existingFilterString,
            string? newFilterToAdd,
            bool clearFilters,
            HttpRequest request,
            string cookieName,
            string? defaultFilterValue = null,
            IEnumerable<FilterModel>? availableFilters = null
        )
        {
            var cookieHasBeenSet = request.Cookies.ContainsKey(cookieName);
            var noFiltersInQueryParams = existingFilterString == null && newFilterToAdd == null;

            if (clearFilters)
            {
                return null;
            }

            if (cookieHasBeenSet && noFiltersInQueryParams)
            {
                return request.Cookies[cookieName] == EmptyFiltersCookieValue ? null : GetValidFilters(request.Cookies[cookieName], availableFilters);
            }

            var filterString = noFiltersInQueryParams
                                ? defaultFilterValue
                                : AddNewFilterToFilterString(existingFilterString, newFilterToAdd);

            return GetValidFilters(filterString, availableFilters);
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

        public static IEnumerable<FilterOptionModel> GetPromptFilterOptions(Prompt prompt)
        {
            return prompt.Options.Count > 0
                ? GetFilterOptionsForPromptWithOptions(prompt)
                : GetFilterOptionsForPromptWithoutOptions(prompt);
        }

        public static string GetFilterValueForAdminField(
            CourseAdminFieldWithAnswer courseAdminFieldWithAnswer
        )
        {
            var group = GetFilterGroupForAdminField(courseAdminFieldWithAnswer.PromptNumber, courseAdminFieldWithAnswer.PromptText);

            string propertyValue;

            if (courseAdminFieldWithAnswer.Options.Any())
            {
                propertyValue = string.IsNullOrEmpty(courseAdminFieldWithAnswer.Answer)
                    ? EmptyValue
                    : courseAdminFieldWithAnswer.Answer;
            }
            else
            {
                propertyValue = string.IsNullOrEmpty(courseAdminFieldWithAnswer.Answer)
                    ? FreeTextBlankValue
                    : FreeTextNotBlankValue;
            }

            return BuildFilterValueString(group, group.Split('(')[0], propertyValue);
        }

        public static string GetFilterValueForRegistrationPrompt(int promptNumber, string? answer, string prompt)
        {
            var group = GetFilterGroupForRegistrationPrompt(promptNumber, prompt);
            var propertyValue = string.IsNullOrEmpty(answer)
                ? EmptyValue
                : answer;
            return BuildFilterValueString(group, group.Split('(')[0], propertyValue);
        }

        public static string? GetValidFilters(string? existingFilterString, IEnumerable<FilterModel>? availableFilters)
        {
            if (string.IsNullOrEmpty(existingFilterString) || availableFilters == null)
            {
                return null;
            }
            var existingFilters = existingFilterString.Split(FilterSeparator);
            var validFilterValues = availableFilters
                                    .SelectMany(filter => filter.FilterOptions)
                                    .Select(option => option.FilterValue)
                                    .ToHashSet();

            var filteredResults = existingFilters
                                    .Where(entry => IsFilterInvalid(entry, validFilterValues))
                                    .ToList();
            var newFilterString = string.Join(FilterSeparator, filteredResults);

            return string.IsNullOrEmpty(newFilterString) ? null : newFilterString;
        }

        private static bool IsFilterInvalid(string filterEntry, HashSet<string> validFilterValues)
        {
            if (validFilterValues.Contains(filterEntry)) return true;
            return false;
        }
        public static string RemoveDuplicateFilters(string? existingFilterString, string newFilterToAdd)
        {
            if (string.IsNullOrEmpty(existingFilterString))
            {
                return existingFilterString ?? string.Empty;
            }
            var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();
            if (!string.IsNullOrEmpty(newFilterToAdd))
            {
                var filterHeader = newFilterToAdd.Split(FilteringHelper.Separator)[0];
                var dupfilters = selectedFilters.Where(x => x.Contains(filterHeader));
                if (dupfilters.Count() > 1)
                {
                    foreach (var filter in selectedFilters)
                    {
                        if (filter.Contains(filterHeader))
                        {
                            selectedFilters.Remove(filter);
                            existingFilterString = string.Join(FilteringHelper.FilterSeparator, selectedFilters);
                            break;
                        }
                    }
                }
            }
            return existingFilterString;
        }
        private static IEnumerable<FilterOptionModel> GetFilterOptionsForPromptWithOptions(Prompt prompt)
        {
            var group = GetFilterGroupForPrompt(prompt);

            var options = prompt.Options.Select(
                option => new FilterOptionModel(
                    option.Trim(),
                    BuildFilterValueString(group, group.Split('(')[0], option.Trim()),
                    FilterStatus.Default
                )
            ).ToList();
            options.Add(
                new FilterOptionModel(
                    "No option selected",
                    BuildFilterValueString(
                        group,
                        group.Split('(')[0],
                        EmptyValue
                    ),
                    FilterStatus.Default
                )
            );
            return options;
        }

        private static IEnumerable<FilterOptionModel> GetFilterOptionsForPromptWithoutOptions(Prompt prompt)
        {
            var group = GetFilterGroupForPrompt(prompt);

            var options = new List<FilterOptionModel>
            {
                new FilterOptionModel(
                    "Not blank",
                    BuildFilterValueString(
                        group,
                        group.Split('(')[0],
                        FreeTextNotBlankValue
                    ),
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "Blank",
                    BuildFilterValueString(
                        group,
                        group.Split('(')[0],
                        FreeTextBlankValue
                    ),
                    FilterStatus.Default
                ),
            };
            return options;
        }

        private static string GetFilterGroupForPrompt(Prompt prompt)
        {
            // Course Admin Fields and Centre Registration Prompt filters rely on properties of the models
            // called Answer1, Answer2 etc. We append the prompt text in brackets to the property
            // name when setting up these filters so that we can check whether they are valid filters for another course etc.
            // e.g. Answer1(Access Permission)|Answer1|FREETEXTBLANKVALUE would not be a valid filter for
            // a course with admin field "Priority Access" at position 1, or a course with no admin field at position 1,
            // but would technically be able to be used for filtering the models.
            return prompt is CentreRegistrationPrompt registrationPrompt
                ? GetFilterGroupForRegistrationPrompt(registrationPrompt.RegistrationField.Id, prompt.PromptText)
                : GetFilterGroupForAdminField(((CourseAdminField)prompt).PromptNumber, prompt.PromptText);
        }

        private static string GetFilterGroupForRegistrationPrompt(int promptNumber, string promptText)
        {
            return
                $"{DelegateUserCard.GetPropertyNameForDelegateRegistrationPromptAnswer(promptNumber)}({promptText})";
        }

        private static string GetFilterGroupForAdminField(int promptNumber, string promptText)
        {
            return $"{CourseDelegate.GetPropertyNameForAdminFieldAnswer(promptNumber)}({promptText})";
        }

        private static bool AvailableFiltersContainsAllSelectedFilters(FilterOptions filterOptions)
        {
            var currentFilters = filterOptions.FilterString?.Split(FilterSeparator).ToList() ??
                                 new List<string>();

            return currentFilters.All(filter => AvailableFiltersContainsFilter(filterOptions.AvailableFilters, filter));
        }

        private static bool AvailableFiltersContainsFilter(IEnumerable<FilterModel> availableFilters, string filter)
        {
            return availableFilters.Any(filterModel => FilterOptionsContainsFilter(filter, filterModel.FilterOptions!));
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
                PropertyName = splitFilter[1];
                PropertyValue = splitFilter[2];
            }

            public string Group { get; }

            public string PropertyName { get; }

            public string PropertyValue { get; }
        }
    }
}
