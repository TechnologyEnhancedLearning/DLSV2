namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using FluentMigrator.Runner.Extensions;

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
                tags.Add(new SearchableTagViewModel(CourseVisibilityFilterOptions.IsHiddenInLearningPortal));
            }
            else
            {
                tags.Add(new SearchableTagViewModel(CourseVisibilityFilterOptions.IsNotHiddenInLearningPortal));
            }

            return tags;
        }

        public static IEnumerable<SearchableTagViewModel> GetCurrentTagsForCourseDelegate(CourseDelegate courseDelegate)
        {
            var tags = new List<SearchableTagViewModel>();

            if (courseDelegate.Active)
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateAccountStatusFilterOptions.Active));
            }
            else
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateAccountStatusFilterOptions.Inactive));
            }

            if (courseDelegate.Locked)
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateProgressLockedFilterOptions.Locked));
            }
            else
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateProgressLockedFilterOptions.NotLocked, true));
            }

            return tags;
        }
    }
}
