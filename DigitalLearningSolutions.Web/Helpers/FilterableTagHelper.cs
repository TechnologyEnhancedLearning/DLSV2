namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
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

            if(adminUser.IsNominatedSupervisor)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.NominatedSupervisor));
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

        public static IEnumerable<SearchableTagViewModel> GetCurrentTagsForDelegateCourses(
            CourseStatistics courseStatistics
        )
        {
            return new List<SearchableTagViewModel>
            {
                courseStatistics.Active
                    ? new SearchableTagViewModel(CourseStatusFilterOptions.IsActive)
                    : new SearchableTagViewModel(CourseStatusFilterOptions.IsInactive),
            };
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

            if (courseDelegate.RemovedDate.HasValue)
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateProgressRemovedFilterOptions.Removed));
            }
            else
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateProgressRemovedFilterOptions.NotRemoved, true));
            }

            //TODO HEEDLS-838 check precedence for these
            if (courseDelegate.Completed.HasValue)
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateCompletionFilterOptions.Complete));
            }
            else if (!courseDelegate.Completed.HasValue && !courseDelegate.Removed)
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateCompletionFilterOptions.Incomplete));
            }
            else if (!courseDelegate.Completed.HasValue && courseDelegate.Removed)
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateCompletionFilterOptions.Removed));
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
                ),
            };
        }

        public static IEnumerable<SearchableTagViewModel> GetCurrentTagsForCourse(CourseAssessmentDetails details)
        {
            return new List<SearchableTagViewModel>
            {
                details.IsAssessed
                    ? new SearchableTagViewModel(AddCourseToGroupAssessedFilterOptions.IsAssessed)
                    : new SearchableTagViewModel(AddCourseToGroupAssessedFilterOptions.IsNotAssessed),
                details.HasLearning
                    ? new SearchableTagViewModel(AddCourseToGroupLearningFilterOptions.HasLearning)
                    : new SearchableTagViewModel(AddCourseToGroupLearningFilterOptions.NoLearning, true),
                details.IsAssessed
                    ? new SearchableTagViewModel(AddCourseToGroupDiagnosticFilterOptions.HasDiagnostic)
                    : new SearchableTagViewModel(AddCourseToGroupDiagnosticFilterOptions.NoDiagnostic, true),
            };
        }
    }
}
