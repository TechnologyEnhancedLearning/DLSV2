namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public static class FilterableTagHelper
    {
        public static IEnumerable<SearchableTagViewModel> GetCurrentTagsForAdminUser(AdminUser adminUser)
        {
            var tags = new List<SearchableTagViewModel>();
            
            if (adminUser.IsLocked)
            {
                tags.Add(new SearchableTagViewModel(AdminFilterOptions.IsLocked));
            }
            else
            {
                tags.Add(new SearchableTagViewModel(AdminFilterOptions.IsNotLocked, true));
            }

            if (adminUser.IsCentreAdmin)
            {
                tags.Add(new SearchableTagViewModel(AdminFilterOptions.CentreAdministrator));
            }

            if (adminUser.IsSupervisor)
            {
                tags.Add(new SearchableTagViewModel(AdminFilterOptions.Supervisor));
            }

            if (adminUser.IsTrainer)
            {
                tags.Add(new SearchableTagViewModel(AdminFilterOptions.Trainer));
            }

            if (adminUser.IsContentCreator)
            {
                tags.Add(new SearchableTagViewModel(AdminFilterOptions.ContentCreatorLicense));
            }

            if (adminUser.IsCmsAdministrator)
            {
                tags.Add(new SearchableTagViewModel(AdminFilterOptions.CmsAdministrator));
            }

            if (adminUser.IsContentManager)
            {
                tags.Add(new SearchableTagViewModel(AdminFilterOptions.CmsManager));
            }

            return tags;
        }
    }
}
