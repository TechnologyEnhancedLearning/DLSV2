namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public static class FilteringHelper
    {
        public const char Separator = '|';

        public static IEnumerable<T> FilterItems<T>(
            IQueryable<T> items,
            string? filterBy
        ) where T : BaseSearchableItem
        {
            var listOfFilters = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(filterBy);

            foreach (var filter in listOfFilters)
            {
                var splitFilter = filter.Split(Separator);
                var propertyName = splitFilter[0];
                var propertyValueString = splitFilter[1];
                var propertyType = typeof(T).GetProperty(propertyName)!.PropertyType;
                var propertyValue = TypeDescriptor.GetConverter(propertyType).ConvertFromString(propertyValueString);

                items = items.Where(propertyName, propertyValue);
            }

            return items;
        }
    }

    public static class AdminFilterOptions
    {
        public static readonly FilterOptionViewModel CentreAdministrator = new FilterOptionViewModel(
            "Centre administrator",
            nameof(AdminUser.IsCentreAdmin) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel Supervisor = new FilterOptionViewModel(
            "Supervisor",
            nameof(AdminUser.IsSupervisor) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel Trainer = new FilterOptionViewModel(
            "Trainer",
            nameof(AdminUser.IsTrainer) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel ContentCreatorLicense =
            new FilterOptionViewModel(
                "Content Creator license",
                nameof(AdminUser.IsContentCreator) + FilteringHelper.Separator + "true",
                FilterStatus.Default
            );

        public static readonly FilterOptionViewModel CmsAdministrator =
            new FilterOptionViewModel(
                "CMS administrator",
                nameof(AdminUser.IsCmsAdministrator) + FilteringHelper.Separator + "true",
                FilterStatus.Default
            );

        public static readonly FilterOptionViewModel CmsManager = new FilterOptionViewModel(
            "CMS manager",
            nameof(AdminUser.IsContentManager) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel IsLocked = new FilterOptionViewModel(
            "Locked",
            nameof(AdminUser.IsLocked) + FilteringHelper.Separator + "true",
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel IsNotLocked = new FilterOptionViewModel(
            "Not locked",
            nameof(AdminUser.IsLocked) + FilteringHelper.Separator + "false",
            FilterStatus.Default
        );
    }
}
