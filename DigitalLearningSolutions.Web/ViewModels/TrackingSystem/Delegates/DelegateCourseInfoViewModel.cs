namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class DelegateCourseInfoViewModel
    {
        private const string DateFormat = "dd/MM/yyyy";

        public DelegateCourseInfoViewModel(
            DelegateCourseInfo info,
            List<CustomPromptWithAnswer>? courseCustomPromptsWithAnswers,
            (int totalAttempts, int attemptsPassed) attemptStats
        )
        {
            CustomisationId = info.CustomisationId;
            ApplicationName = info.ApplicationName;
            CustomisationName = info.CustomisationName;
            SupervisorForename = info.SupervisorForename;
            SupervisorSurname = info.SupervisorSurname;
            Enrolled = info.Enrolled.ToString(DateFormat);
            LastUpdated = info.LastUpdated.ToString(DateFormat);
            CompleteBy = info.CompleteBy?.ToString(DateFormat);
            Completed = info.Completed?.ToString(DateFormat);
            Evaluated = info.Evaluated?.ToString(DateFormat);
            EnrolmentMethod = info.EnrolmentMethodId switch
            {
                1 => "Self enrolled",
                2 => "Administrator",
                3 => "Group",
                4 => "System",
                _ => ""
            };
            LoginCount = info.LoginCount;
            LearningTime = info.LearningTime + " mins";
            DiagnosticScore = info.DiagnosticScore;
            IsAssessed = info.IsAssessed;

            CourseCustomPromptsWithAnswers = courseCustomPromptsWithAnswers ?? new List<CustomPromptWithAnswer>();
            TotalAttempts = attemptStats.totalAttempts;
            AttemptsPassed = attemptStats.attemptsPassed;
        }

        public int CustomisationId { get; set; }
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

        public List<CustomPromptWithAnswer> CourseCustomPromptsWithAnswers { get; set; }
        public int TotalAttempts { get; set; }
        public int AttemptsPassed { get; set; }

        public string? Supervisor
        {
            get
            {
                // SupervisorSurname is not nullable in db; will only be null if no supervisor
                if (SupervisorSurname == null)
                {
                    return null;
                }

                return (string.IsNullOrEmpty(SupervisorForename) ? "" : $"{SupervisorForename} ") + SupervisorSurname;
            }
        }

        public string CourseName =>
            ApplicationName + (string.IsNullOrEmpty(CustomisationName) ? "" : $" - {CustomisationName}");

        public string? PassRate =>
            TotalAttempts != 0 ? Math.Round(100 * AttemptsPassed / (double)TotalAttempts) + "%" : null;
    }
}
