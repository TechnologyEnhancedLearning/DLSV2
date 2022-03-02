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
            ApplicationName = info.ApplicationName;
            CustomisationName = info.CustomisationName;
            SupervisorForename = info.SupervisorForename;
            SupervisorSurname = info.SupervisorSurname;
            Enrolled = info.Enrolled.ToString(DateHelper.StandardDateFormat);
            LastUpdated = info.LastUpdated.ToString(DateHelper.StandardDateFormat);
            CompleteBy = info.CompleteBy?.ToString(DateHelper.StandardDateFormat);
            Completed = info.Completed?.ToString(DateHelper.StandardDateFormat);
            Evaluated = info.Evaluated?.ToString(DateHelper.StandardDateFormat);
            IsProgressLocked = info.IsProgressLocked;
            EnrolmentMethod = info.EnrolmentMethodId switch
            {
                1 => "Self enrolled",
                2 => "Administrator",
                3 => "Group",
                4 => "System",
                _ => "",
            };
            LoginCount = info.LoginCount;
            LearningTime = info.LearningTime + " mins";
            DiagnosticScore = info.DiagnosticScore;
            IsAssessed = info.IsAssessed;

            DelegateId = details.DelegateCourseInfo.DelegateId;

            CourseAdminFieldsWithAnswers = details.CourseAdminFields;
            TotalAttempts = details.AttemptStats.TotalAttempts;
            AttemptsPassed = details.AttemptStats.AttemptsPassed;
            PassRate = details.AttemptStats.PassRate;
        }

        public int ProgressId { get; set; }
        public int CustomisationId { get; set; }
        public int DelegateId { get; set; }
        public string ApplicationName { get; set; }
        public string CustomisationName { get; set; }
        public string? SupervisorForename { get; set; }
        public string? SupervisorSurname { get; set; }
        public string Enrolled { get; set; }
        public string LastUpdated { get; set; }
        public string? CompleteBy { get; set; }
        public string? Completed { get; set; }
        public string? Evaluated { get; set; }
        public string EnrolmentMethod { get; set; }
        public int LoginCount { get; set; }
        public string LearningTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public bool IsAssessed { get; set; }

        public List<CourseAdminFieldWithAnswer> CourseAdminFieldsWithAnswers { get; set; }
        public int TotalAttempts { get; set; }
        public int AttemptsPassed { get; set; }
        public double PassRate { get; set; }
        public bool IsProgressLocked { get; set; }

        // SupervisorSurname is not nullable in db; will only be null if no supervisor
        public string Supervisor => SupervisorSurname == null
            ? "None"
            : DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(SupervisorForename, SupervisorSurname);

        public string CourseName =>
            ApplicationName + (string.IsNullOrEmpty(CustomisationName) ? "" : $" - {CustomisationName}");

        public string? PassRateDisplayString =>
            TotalAttempts != 0 ? PassRate + "%" : null;
    }
}
