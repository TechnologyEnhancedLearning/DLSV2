namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Helpers;

    public class DelegateCourseInfoViewModel
    {
        public DelegateCourseInfoViewModel(DelegateCourseDetails details)
        {
            var info = details.DelegateCourseInfo;

            ProgressId = info.ProgressId;
            CustomisationId = info.CustomisationId;
            DelegateId = details.DelegateCourseInfo.DelegateId;
            ApplicationName = info.ApplicationName;
            CustomisationName = info.CustomisationName;

            Supervisor = info.SupervisorSurname != null
                ? DisplayStringHelper.GetPotentiallyInactiveAdminName(
                    info.SupervisorForename,
                    info.SupervisorSurname,
                    info.SupervisorAdminActive!.Value
                )
                : "None";

            Enrolled = info.Enrolled.ToString(DateHelper.StandardDateAndTimeFormat);
            EnrolledByFullName = DisplayStringHelper.GetPotentiallyInactiveAdminName(
                details.DelegateCourseInfo.EnrolledByForename,
                details.DelegateCourseInfo.EnrolledBySurname,
                details.DelegateCourseInfo.EnrolledByAdminActive
            );

            CompleteBy = info.CompleteBy?.ToString(DateHelper.StandardDateAndTimeFormat);
            LastUpdated = info.LastUpdated.ToString(DateHelper.StandardDateAndTimeFormat);
            Completed = info.Completed?.ToString(DateHelper.StandardDateAndTimeFormat);
            Evaluated = info.Evaluated?.ToString(DateHelper.StandardDateAndTimeFormat);
            EnrolmentMethod = info.EnrolmentMethodId switch
            {
                1 => "Self",
                2 => "Enrolled by " + EnrolledByFullName,
                3 => "Group",
                _ => "System",
            };
            LoginCount = info.LoginCount;
            LearningTime = info.LearningTime + " mins";
            CourseAdminFieldsWithAnswers = details.CourseAdminFields;

            DiagnosticScore = info.DiagnosticScore;
            IsAssessed = info.IsAssessed;
            IsProgressLocked = info.IsProgressLocked;
            TotalAttempts = details.AttemptStats.TotalAttempts;
            AttemptsPassed = details.AttemptStats.AttemptsPassed;
            PassRate = details.AttemptStats.PassRate;
        }

        public int ProgressId { get; set; }
        public int CustomisationId { get; set; }
        public int DelegateId { get; set; }
        public string ApplicationName { get; set; }
        public string CustomisationName { get; set; }

        public string? Supervisor { get; set; }
        public string Enrolled { get; set; }
        public string? EnrolledByFullName { get; set; }
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
        
        public string CourseName =>
            ApplicationName + (string.IsNullOrEmpty(CustomisationName) ? "" : $" - {CustomisationName}");

        public string? PassRateDisplayString =>
            TotalAttempts != 0 ? PassRate + "%" : null;
    }
}
