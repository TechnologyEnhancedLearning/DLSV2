namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate
{
    using System;
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
        public DelegateCourseInfoViewModel(DelegateCourseDetails details, DelegateAccessRoute accessedVia, ReturnPageQuery? returnPageQuery)
        {
            var info = details.DelegateCourseInfo;
            AccessedVia = accessedVia;
            ProgressId = info.ProgressId;
            CustomisationId = info.CustomisationId;
            DelegateId = details.DelegateCourseInfo.DelegateId;
            CourseName = info.CourseName;

            Supervisor = info.SupervisorSurname != null
                ? DisplayStringHelper.GetPotentiallyInactiveAdminName(
                    info.SupervisorForename,
                    info.SupervisorSurname,
                    info.SupervisorAdminActive!.Value
                )
                : "None";

            Enrolled = info.Enrolled.ToString(DateHelper.StandardDateAndTimeFormat);
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

            CompleteBy = info.CompleteBy?.ToString(DateHelper.StandardDateAndTimeFormat);
            LastUpdated = info.LastUpdated.ToString(DateHelper.StandardDateAndTimeFormat);
            Completed = info.Completed?.ToString(DateHelper.StandardDateAndTimeFormat);
            Evaluated = info.Evaluated?.ToString(DateHelper.StandardDateAndTimeFormat);
            
            LoginCount = info.LoginCount;
            LearningTime = info.LearningTime + " mins";
            CourseAdminFieldsWithAnswers = details.CourseAdminFields;

            DiagnosticScore = info.DiagnosticScore;
            IsAssessed = info.IsAssessed;
            IsProgressLocked = info.IsProgressLocked;
            TotalAttempts = details.AttemptStats.TotalAttempts;
            AttemptsPassed = details.AttemptStats.AttemptsPassed;
            PassRate = details.AttemptStats.PassRate;
            ReturnPageQuery = returnPageQuery;
        }

        public DelegateCourseInfoViewModel(
            CourseDelegate courseDelegate,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery returnPageQuery,
            string? courseName = null
        )
        {
            AccessedVia = accessedVia;
            ProgressId = courseDelegate.ProgressId;
            CustomisationId = courseDelegate.CustomisationId;
            DelegateId = courseDelegate.DelegateId;
            CourseName = courseName;

            CandidateNumber = courseDelegate.CandidateNumber;
            ProfessionalRegistrationNumber = PrnStringHelper.GetPrnDisplayString(
                courseDelegate.HasBeenPromptedForPrn,
                courseDelegate.ProfessionalRegistrationNumber
            );

            CourseDelegatesTitleName = courseDelegate.FullNameForSearchingSorting;
            Email = courseDelegate.EmailAddress;

            Supervisor = courseDelegate.SupervisorSurname != null
                ? DisplayStringHelper.GetPotentiallyInactiveAdminName(
                    courseDelegate.SupervisorForename,
                    courseDelegate.SupervisorSurname,
                    courseDelegate.SupervisorAdminActive!.Value
                )
                : "None";

            Enrolled = courseDelegate.Enrolled.ToString(DateHelper.StandardDateAndTimeFormat);
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

            CompleteBy = courseDelegate.CompleteByDate?.ToString(DateHelper.StandardDateAndTimeFormat);
            LastUpdated = courseDelegate.LastUpdated.ToString(DateHelper.StandardDateAndTimeFormat);
            Completed = courseDelegate.Completed?.ToString(DateHelper.StandardDateAndTimeFormat);
            Evaluated = courseDelegate.Evaluated?.ToString(DateHelper.StandardDateAndTimeFormat);
            
            LoginCount = courseDelegate.LoginCount;
            LearningTime = courseDelegate.LearningTime + " mins";
            CourseAdminFieldsWithAnswers = courseDelegate.CourseAdminFields;

            DiagnosticScore = courseDelegate.DiagnosticScore;
            IsAssessed = courseDelegate.IsAssessed;
            IsProgressLocked = courseDelegate.Locked;
            TotalAttempts = courseDelegate.AllAttempts;
            AttemptsPassed = courseDelegate.AttemptsPassed;
            PassRate = courseDelegate.PassRate;

            RemovedDate = courseDelegate.RemovedDate?.ToString(DateHelper.StandardDateAndTimeFormat);
            ReturnPageQuery = returnPageQuery;
            Tags = FilterableTagHelper.GetCurrentTagsForCourseDelegate(courseDelegate);
        }

        public string? Email { get; set; }

        public string CourseDelegatesTitleName { get; set; }
        public string CandidateNumber { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public DelegateAccessRoute AccessedVia { get; set; }
        public int ProgressId { get; set; }
        public int CustomisationId { get; set; }
        public int DelegateId { get; set; }

        public string? Supervisor { get; set; }
        public string Enrolled { get; set; }
        public string? CompleteBy { get; set; }
        public string LastUpdated { get; set; }
        public string? Completed { get; set; }
        public string? Evaluated { get; set; }
        public string EnrolmentMethod { get; set; }
        public int LoginCount { get; set; }
        public string LearningTime { get; set; }
        public List<CourseAdminFieldWithAnswer> CourseAdminFieldsWithAnswers { get; set; }

        public int? DiagnosticScore { get; set; }
        public bool IsAssessed { get; set; }
        public int TotalAttempts { get; set; }
        public int AttemptsPassed { get; set; }
        public double PassRate { get; set; }
        public bool IsProgressLocked { get; set; }

        public string? CourseName { get; set; }
        public string? RemovedDate { get; set; }
        public ReturnPageQuery? ReturnPageQuery { get; set; }
    }
}
