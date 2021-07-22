namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class DelegateCourseInfoViewModel
    {
        public DelegateCourseInfoViewModel(
            DelegateCourseInfo info,
            List<CustomPromptWithAnswer>? customPromptsWithAnswers,
            (int totalAttempts, int attemptsPassed) attemptStats
        )
        {
            CustomisationId = info.CustomisationId;
            ApplicationName = info.ApplicationName;
            CustomisationName = info.CustomisationName;
            Supervisor = info.Supervisor;
            Enrolled = info.Enrolled.ToString("dd/MM/yyyy");
            LastUpdated = info.LastUpdated.ToString("dd/MM/yyyy");
            CompleteBy = info.CompleteBy?.ToString("dd/MM/yyyy");
            Completed = info.Completed?.ToString("dd/MM/yyyy");
            Evaluated = info.Evaluated?.ToString("dd/MM/yyyy");
            EnrollmentMethod = info.EnrollmentMethodId switch
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

            CustomPromptsWithAnswers = customPromptsWithAnswers ?? new List<CustomPromptWithAnswer>();
            TotalAttempts = attemptStats.totalAttempts;
            AttemptsPassed = attemptStats.attemptsPassed;
        }

        public int CustomisationId { get; set; }
        public string ApplicationName { get; set; }
        public string CustomisationName { get; set; }
        public string? Supervisor { get; set; }
        public string Enrolled { get; set; }
        public string LastUpdated { get; set; }
        public string? CompleteBy { get; set; }
        public string? Completed { get; set; }
        public string? Evaluated { get; set; }
        public string EnrollmentMethod { get; set; }
        public int LoginCount { get; set; }
        public string LearningTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public bool IsAssessed { get; set; }

        public List<CustomPromptWithAnswer> CustomPromptsWithAnswers { get; set; }
        public int TotalAttempts { get; set; }
        public int AttemptsPassed { get; set; }

        public double? PassRate =>
            TotalAttempts != 0 ? Math.Round(100 * AttemptsPassed / (double)TotalAttempts) : (double?)null;
    }
}
