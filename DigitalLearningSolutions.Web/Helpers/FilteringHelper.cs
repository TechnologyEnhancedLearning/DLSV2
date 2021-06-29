namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;

    public static class FilteringHelper
    {
        public static IEnumerable<T> FilterItems<T>(
            IQueryable<T> items,
            string? filterBy
        ) where T : BaseSearchableItem
        {
            var listOfFilters = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(filterBy);

            foreach (var filter in listOfFilters)
            {
                var splitFilter = filter.Split('|');
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
        public static readonly (string DisplayText, string Filter) CentreAdministrator =
            ("Centre administrator", nameof(AdminUser.IsCentreAdmin) + "|true");
        public static readonly (string DisplayText, string Filter) Supervisor =
            ("Supervisor", nameof(AdminUser.IsSupervisor) + "|true");
        public static readonly (string DisplayText, string Filter) Trainer =
            ("Trainer", nameof(AdminUser.IsTrainer) + "|true");
        public static readonly (string DisplayText, string Filter) ContentCreatorLicense =
            ("Content Creator license", nameof(AdminUser.IsContentCreator) + "|true");
        public static readonly (string DisplayText, string Filter) CmsAdministrator =
            ("CMS administrator", nameof(AdminUser.IsCmsAdministrator) + "|true");
        public static readonly (string DisplayText, string Filter) CmsManager =
            ("CMS manager", nameof(AdminUser.IsContentManager) + "|true");
        public static readonly (string DisplayText, string Filter) IsLocked =
            ("Locked", nameof(AdminUser.IsLocked) + "|true");
        public static readonly (string DisplayText, string Filter) IsNotLocked =
            ("Not locked", nameof(AdminUser.IsLocked) + "|false");
    }
}
