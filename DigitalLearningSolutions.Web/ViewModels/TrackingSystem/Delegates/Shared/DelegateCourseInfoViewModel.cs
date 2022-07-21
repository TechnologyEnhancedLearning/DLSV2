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
            DelegateCourseInfo delegateCourseInfo,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery? returnPageQuery = null
        ) : this(delegateCourseInfo)
        {
            AccessedVia = accessedVia;
            ReturnPageQuery = returnPageQuery;
        }

        public DelegateCourseInfoViewModel(
            CourseDelegate courseDelegate,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery returnPageQuery
        ) : this(courseDelegate)
        {
            AccessedVia = accessedVia;
            ReturnPageQuery = returnPageQuery;
            Tags = FilterableTagHelper.GetCurrentTagsForCourseDelegate(courseDelegate);
        }

        private DelegateCourseInfoViewModel(DelegateCourseInfo info)
        {
            ProgressId = info.ProgressId;
            CustomisationId = info.CustomisationId;

            DelegateId = info.DelegateId;
            DelegateName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                info.DelegateFirstName,
                info.DelegateLastName
            );
            CourseDelegatesDisplayName =
                NameQueryHelper.GetSortableFullName(info.DelegateFirstName, info.DelegateLastName);
            Email = info.DelegateEmail;
            DelegateNumber = info.CandidateNumber;
            ProfessionalRegistrationNumber = PrnHelper.GetPrnDisplayString(
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
                info.EnrolledByForename,
                info.EnrolledBySurname,
                info.EnrolledByAdminActive
            );
            EnrolmentMethod = info.EnrolmentMethodId switch
            {
                1 => "Self enrolled",
                2 => "Enrolled by " + (enrolledByFullName ?? "Admin"),
                3 => "Group",
                _ => "System",
            };

            LoginCount = info.LoginCount;
            LearningTime = info.LearningTime + " mins";
            DiagnosticScore = info.DiagnosticScore;
            CourseAdminFieldsWithAnswers = info.CourseAdminFields;
            IsAssessed = info.IsAssessed;
            IsProgressLocked = info.IsProgressLocked;
            TotalAttempts = info.AllAttempts;
            AttemptsPassed = info.AttemptsPassed;
            PassRate = info.PassRate;
            CourseName = info.CourseName;
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
        public string? CourseName { get; set; }
        public string CourseDelegatesDisplayName { get; set; }

        public string? PassRateDisplayString =>
            TotalAttempts != 0 ? PassRate + "%" : null;
    }
}
