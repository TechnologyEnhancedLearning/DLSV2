namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class FilterableTagHelper
    {
        public static IEnumerable<SearchableTagViewModel> GetCurrentTagsForAdminUser(AdminUser adminUser)
        {
            var tags = new List<SearchableTagViewModel>();

            if (adminUser.IsLocked)
            {
                tags.Add(new SearchableTagViewModel(AdminAccountStatusFilterOptions.IsLocked));
            }
            else
            {
                tags.Add(new SearchableTagViewModel(AdminAccountStatusFilterOptions.IsNotLocked, true));
            }

            if (adminUser.IsCentreAdmin)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.CentreAdministrator));
            }

            if (adminUser.IsSupervisor)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.Supervisor));
            }

            if (adminUser.IsTrainer)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.Trainer));
            }

            if (adminUser.IsContentCreator)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.ContentCreatorLicense));
            }

            if (adminUser.IsCmsAdministrator)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.CmsAdministrator));
            }

            if (adminUser.IsCmsManager)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.CmsManager));
            }

            return tags;
        }

        public static IEnumerable<SearchableTagViewModel> GetCurrentTagsForCourseStatistics(
            CourseStatistics courseStatistics
        )
        {
            var tags = new List<SearchableTagViewModel>();

            if (courseStatistics.Active)
            {
                tags.Add(new SearchableTagViewModel(CourseStatusFilterOptions.IsActive));
            }
            else
            {
                tags.Add(new SearchableTagViewModel(CourseStatusFilterOptions.IsInactive));
            }

            if (courseStatistics.HideInLearnerPortal)
            {
                tags.Add(new SearchableTagViewModel(CourseStatusFilterOptions.IsHiddenInLearningPortal));
            }
            else
            {
                tags.Add(new SearchableTagViewModel(CourseStatusFilterOptions.IsNotHiddenInLearningPortal));
            }

            return tags;
        }

        public static IEnumerable<SearchableTagViewModel> GetCurrentTagsForDelegateUser(
            DelegateUserCard delegateUser
        )
        {
            return new List<SearchableTagViewModel>
            {
                delegateUser.Active
                    ? new SearchableTagViewModel(DelegateActiveStatusFilterOptions.IsActive)
                    : new SearchableTagViewModel(DelegateActiveStatusFilterOptions.IsNotActive),
                delegateUser.IsPasswordSet
                    ? new SearchableTagViewModel(DelegatePasswordStatusFilterOptions.PasswordSet)
                    : new SearchableTagViewModel(DelegatePasswordStatusFilterOptions.PasswordNotSet),
                delegateUser.IsAdmin
                    ? new SearchableTagViewModel(DelegateAdminStatusFilterOptions.IsAdmin)
                    : new SearchableTagViewModel(DelegateAdminStatusFilterOptions.IsNotAdmin, true),
                new SearchableTagViewModel(
                    DelegateRegistrationTypeFilterOptions.FromRegistrationType(delegateUser.RegistrationType)
                )
            };
        }
    }
}
