namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;

    public static class FilterableTagHelper
    {
        public static IEnumerable<(string, string)> GetCurrentTagsForAdminUser(AdminUser adminUser)
        {
            var tags = new List<(string, string)>();

            // TODO: Either change these when HEEDLS-532 is merged first or change them on HEEDLS-532 if HEEDLS-416 is merged first to the AdminFilterOptions
            if (adminUser.IsLocked)
            {
                tags.Add(("Locked", nameof(AdminUser.IsLocked) + "|true"));
            }

            if (adminUser.IsCentreAdmin)
            {
                tags.Add(("Centre administrator", nameof(AdminUser.IsCentreAdmin) + "|true"));
            }

            if (adminUser.IsSupervisor)
            {
                tags.Add(("Supervisor", nameof(AdminUser.IsSupervisor) + "|true"));
            }

            if (adminUser.IsTrainer)
            {
                tags.Add(("Trainer", nameof(AdminUser.IsTrainer) + "|true"));
            }

            if (adminUser.IsContentCreator)
            {
                tags.Add(("Content Creator license", nameof(AdminUser.IsContentCreator) + "|true"));
            }

            if (adminUser.IsCmsAdministrator)
            {
                tags.Add(("CMS administrator", nameof(AdminUser.IsCmsAdministrator) + "|true"));
            }

            if (adminUser.IsContentManager)
            {
                tags.Add(("CMS manager", nameof(AdminUser.IsContentManager) + "|true"));
            }

            return tags;
        }
    }
}
