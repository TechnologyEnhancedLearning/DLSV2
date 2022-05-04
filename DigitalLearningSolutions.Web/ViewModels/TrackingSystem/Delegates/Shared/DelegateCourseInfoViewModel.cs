namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DateHelper = DigitalLearningSolutions.Web.Helpers.DateHelper;

    public class DelegateCourseInfoViewModel : BaseFilterableViewModel
    {
        public DelegateCourseInfoViewModel(
            DelegateCourseDetails details,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery? returnPageQuery
        )
        {
            var info = details.DelegateCourseInfo;

            AccessedVia = accessedVia;
            ReturnPageQuery = returnPageQuery;
            ProgressId = info.ProgressId;
            CustomisationId = info.CustomisationId;

            DelegateId = details.DelegateCourseInfo.DelegateId;
            DelegateName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                details.DelegateCourseInfo.DelegateFirstName,
                details.DelegateCourseInfo.DelegateLastName
            );
            Email = details.DelegateCourseInfo.DelegateEmail;
            DelegateNumber = details.DelegateCourseInfo.DelegateNumber;
            ProfessionalRegistrationNumber = PrnStringHelper.GetPrnDisplayString(
                info.HasBeenPromptedForPrn,
                info.ProfessionalRegistrationNumber
            );

            Enrolled = info.Enrolled.ToString(DateHelper.StandardDateAndTimeFormat);
            Supervisor = info.SupervisorSurname != null
                ? DisplayStringHelper.GetPotentiallyInactiveAdminName(
                    info.SupervisorForename,
                    info.SupervisorSurname,
                    info.SupervisorAdminActive!.Value
                )
                : "None";

            CompleteBy = info.CompleteBy?.ToString(DateHelper.StandardDateAndTimeFormat);
            LastAccessed = info.LastUpdated.ToString(DateHelper.StandardDateAndTimeFormat);
            Completed = info.Completed?.ToString(DateHelper.StandardDateAndTimeFormat);
            Evaluated = info.Evaluated?.ToString(DateHelper.StandardDateAndTimeFormat);
            RemovedDate = info.RemovedDate?.ToString(DateHelper.StandardDateAndTimeFormat);

            var enrolledByFullName = DisplayStringHelper.GetPotentiallyInactiveAdminName(
                details.DelegateCourseInfo.EnrolledByForename,
                details.DelegateCourseInfo.EnrolledBySurname,
                details.DelegateCourseInfo.EnrolledByAdminActive
            );
            EnrolmentMethod = details.DelegateCourseInfo.EnrolmentMethodId switch
            {
                1 => "Self enrolled",
                2 => "Enrolled by " + enrolledByFullName,
                3 => "Group",
                _ => "System",
            };

            LoginCount = info.LoginCount;
            LearningTime = info.LearningTime + " mins";
            DiagnosticScore = info.DiagnosticScore;
            CourseAdminFieldsWithAnswers = details.CourseAdminFields;
            IsAssessed = info.IsAssessed;
            IsProgressLocked = info.IsProgressLocked;
            TotalAttempts = details.AttemptStats.TotalAttempts;
            AttemptsPassed = details.AttemptStats.AttemptsPassed;
            PassRate = details.AttemptStats.PassRate;

            CourseName = info.CourseName;
        }

        public DelegateCourseInfoViewModel(
            CourseDelegate courseDelegate,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery returnPageQuery,
            string? courseName = null
        )
        {
            AccessedVia = accessedVia;
            ReturnPageQuery = returnPageQuery;
            ProgressId = courseDelegate.ProgressId;
            CustomisationId = courseDelegate.CustomisationId;

            DelegateId = courseDelegate.DelegateId;
            DelegateName = courseDelegate.FullNameForSearchingSorting;
            Email = courseDelegate.EmailAddress;
            DelegateNumber = courseDelegate.CandidateNumber;
            ProfessionalRegistrationNumber = PrnStringHelper.GetPrnDisplayString(
                courseDelegate.HasBeenPromptedForPrn,
                courseDelegate.ProfessionalRegistrationNumber
            );

            Enrolled = courseDelegate.Enrolled.ToString(DateHelper.StandardDateAndTimeFormat);
            Supervisor = courseDelegate.SupervisorSurname != null
                ? DisplayStringHelper.GetPotentiallyInactiveAdminName(
                    courseDelegate.SupervisorForename,
                    courseDelegate.SupervisorSurname,
                    courseDelegate.SupervisorAdminActive!.Value
                )
                : "None";

            CompleteBy = courseDelegate.CompleteByDate?.ToString(DateHelper.StandardDateAndTimeFormat);
            LastAccessed = courseDelegate.LastUpdated.ToString(DateHelper.StandardDateAndTimeFormat);
            Completed = courseDelegate.Completed?.ToString(DateHelper.StandardDateAndTimeFormat);
            Evaluated = courseDelegate.Evaluated?.ToString(DateHelper.StandardDateAndTimeFormat);
            RemovedDate = courseDelegate.RemovedDate?.ToString(DateHelper.StandardDateAndTimeFormat);

            var enrolledByFullName = DisplayStringHelper.GetPotentiallyInactiveAdminName(
                courseDelegate.EnrolledByForename,
                courseDelegate.EnrolledBySurname,
                courseDelegate.EnrolledByAdminActive
            );
            EnrolmentMethod = courseDelegate.EnrolmentMethodId switch
            {
                1 => "Self enrolled",
                2 => "Enrolled by " + enrolledByFullName,
                3 => "Group",
                _ => "System",
            };

            LoginCount = courseDelegate.LoginCount;
            LearningTime = courseDelegate.LearningTime + " mins";
            DiagnosticScore = courseDelegate.DiagnosticScore;
            CourseAdminFieldsWithAnswers = courseDelegate.CourseAdminFields;
            IsAssessed = courseDelegate.IsAssessed;

            TotalAttempts = courseDelegate.AllAttempts;
            AttemptsPassed = courseDelegate.AttemptsPassed;
            PassRate = courseDelegate.PassRate;
            IsProgressLocked = courseDelegate.Locked;

            CourseName = courseName;
            Registered = courseDelegate.Registered.ToString(DateHelper.StandardDateAndTimeFormat);
            Tags = FilterableTagHelper.GetCurrentTagsForCourseDelegate(courseDelegate);
        }

        public DelegateAccessRoute AccessedVia { get; set; }
        public ReturnPageQuery? ReturnPageQuery { get; set; }
        public int ProgressId { get; set; }
        public int CustomisationId { get; set; }
        public int DelegateId { get; set; }
        public string DelegateName { get; set; }
        public string? Email { get; set; }
        public string DelegateNumber { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }

        public string Enrolled { get; set; }
        public string? Supervisor { get; set; }
        public string? CompleteBy { get; set; }
        public string LastAccessed { get; set; }
        public string? Completed { get; set; }
        public string? Evaluated { get; set; }
        public string? RemovedDate { get; set; }
        public string EnrolmentMethod { get; set; }
        public int LoginCount { get; set; }
        public string LearningTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public List<CourseAdminFieldWithAnswer> CourseAdminFieldsWithAnswers { get; set; }
        public bool IsAssessed { get; set; }
        public int TotalAttempts { get; set; }
        public int AttemptsPassed { get; set; }
        public double PassRate { get; set; }
        public bool IsProgressLocked { get; set; }

        // View Delegate only property
        public string? CourseName { get; set; }

        // Course Delegates only properties
        public string Registered { get; set; }
    }
}
