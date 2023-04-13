namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class FilterableTagHelper
    {
        public static IEnumerable<SearchableTagViewModel> GetCurrentTagsForAdmin(AdminEntity admin)
        {
            var tags = new List<SearchableTagViewModel>();

            if (admin.UserAccount.FailedLoginCount >= AuthHelper.FailedLoginThreshold)
            {
                tags.Add(new SearchableTagViewModel(AdminAccountStatusFilterOptions.IsLocked));
            }
            else
            {
                tags.Add(new SearchableTagViewModel(AdminAccountStatusFilterOptions.IsNotLocked, true));
            }

            if (admin.AdminAccount.IsCentreManager)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.CentreManager));
            }

            if (admin.AdminAccount.IsCentreAdmin)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.CentreAdministrator));
            }

            if (admin.AdminAccount.IsSupervisor)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.Supervisor));
            }

            if (admin.AdminAccount.IsNominatedSupervisor)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.NominatedSupervisor));
            }

            if (admin.AdminAccount.IsTrainer)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.Trainer));
            }

            if (admin.AdminAccount.IsContentCreator)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.ContentCreatorLicense));
            }

            if (admin.AdminAccount.IsCmsAdministrator)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.CmsAdministrator));
            }

            if (admin.AdminAccount.IsCmsManager)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.CmsManager));
            }

            if (admin.AdminAccount.IsSuperAdmin)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.SuperAdmin));
            }

            if (admin.AdminAccount.IsReportsViewer)
            {
                tags.Add(new SearchableTagViewModel(AdminRoleFilterOptions.ReportsViewer));
            }

            return tags;
        }

        public static IEnumerable<SearchableTagViewModel> GetCurrentTagsForCourseStatistics(
            CourseStatistics courseStatistics
        )
        {
            var tags = new List<SearchableTagViewModel>();

            if (courseStatistics.Archived)
            {
                tags.Add(new SearchableTagViewModel("Archived", string.Empty, CourseStatusFilterOptions.IsArchived.TagStatus));
            }
            else if (courseStatistics.Active)
            {
                tags.Add(new SearchableTagViewModel("Active", string.Empty, CourseStatusFilterOptions.IsActive.TagStatus));
            }
            else
            {
                tags.Add(new SearchableTagViewModel("Inactive", string.Empty, CourseStatusFilterOptions.IsInactive.TagStatus));
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
            var tags = new List<SearchableTagViewModel>();

            if (courseStatistics.Archived)
            {
                tags.Add(new SearchableTagViewModel(CourseStatusFilterOptions.IsArchived));
            }
            else if (courseStatistics.Active)
            {
                tags.Add(new SearchableTagViewModel(CourseStatusFilterOptions.IsActive));
            }
            else
            {
                tags.Add(new SearchableTagViewModel(CourseStatusFilterOptions.IsInactive));
            }

            return tags;
        }

        public static IEnumerable<SearchableTagViewModel> GetCurrentStatusTagsForDelegateCourses(
            CourseStatistics courseStatistics
        )
        {
            var tags = new List<SearchableTagViewModel>();

            if (courseStatistics.Archived)
            {
                tags.Add(new SearchableTagViewModel("Archived", string.Empty, CourseStatusFilterOptions.IsArchived.TagStatus));
            }
            else if (courseStatistics.Active)
            {
                tags.Add(new SearchableTagViewModel("Active", string.Empty, CourseStatusFilterOptions.IsActive.TagStatus));
            }
            else
            {
                tags.Add(new SearchableTagViewModel("Inactive", string.Empty, CourseStatusFilterOptions.IsInactive.TagStatus));
            }

            return tags;
        }

        public static IEnumerable<SearchableTagViewModel> GetCurrentTagsForCourseDelegate(CourseDelegate courseDelegate)
        {
            var tags = new List<SearchableTagViewModel>();

            if (courseDelegate.IsDelegateActive)
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateAccountStatusFilterOptions.Active));
            }
            else
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateAccountStatusFilterOptions.Inactive));
            }

            if (courseDelegate.IsProgressLocked)
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

            if (courseDelegate.HasCompleted)
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateCompletionFilterOptions.Complete));
            }
            else
            {
                tags.Add(new SearchableTagViewModel(CourseDelegateCompletionFilterOptions.Incomplete));
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
