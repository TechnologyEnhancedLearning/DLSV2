namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class FilteringHelper
    {
        public const char Separator = '|';

        public static IEnumerable<T> FilterItems<T>(
            IQueryable<T> items,
            string? filterBy
        ) where T : BaseSearchableItem
        {
            var listOfFilters = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(filterBy);

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
            return items.Where(propertyName, propertyValue);
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

    public static class AdminRoleFilterOptions
    {
        private const string Category = "Role";

        public static readonly FilterOptionViewModel CentreAdministrator = new FilterOptionViewModel(
            "Centre administrator",
            Category + FilteringHelper.Separator + nameof(AdminUser.IsCentreAdmin) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel Supervisor = new FilterOptionViewModel(
            "Supervisor",
            Category + FilteringHelper.Separator + nameof(AdminUser.IsSupervisor) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel Trainer = new FilterOptionViewModel(
            "Trainer",
            Category + FilteringHelper.Separator + nameof(AdminUser.IsTrainer) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel ContentCreatorLicense =
            new FilterOptionViewModel(
                "Content Creator license",
                Category + FilteringHelper.Separator + nameof(AdminUser.IsContentCreator) + FilteringHelper.Separator +
                "true",
                FilterStatus.Default
            );

        public static readonly FilterOptionViewModel CmsAdministrator =
            new FilterOptionViewModel(
                "CMS administrator",
                Category + FilteringHelper.Separator + nameof(AdminUser.IsCmsAdministrator) +
                FilteringHelper.Separator + "true",
                FilterStatus.Default
            );

        public static readonly FilterOptionViewModel CmsManager = new FilterOptionViewModel(
            "CMS manager",
            Category + FilteringHelper.Separator + nameof(AdminUser.IsCmsManager) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );
    }

    public static class AdminAccountStatusFilterOptions
    {
        private const string Category = "AccountStatus";

        public static readonly FilterOptionViewModel IsLocked = new FilterOptionViewModel(
            "Locked",
            Category + FilteringHelper.Separator + nameof(AdminUser.IsLocked) + FilteringHelper.Separator + "true",
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel IsNotLocked = new FilterOptionViewModel(
            "Not locked",
            Category + FilteringHelper.Separator + nameof(AdminUser.IsLocked) + FilteringHelper.Separator + "false",
            FilterStatus.Default
        );
    }
}
