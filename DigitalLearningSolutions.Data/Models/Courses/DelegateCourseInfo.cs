namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public class DelegateCourseInfo
    {
        public DelegateCourseInfo() { }

        public DelegateCourseInfo(
            string applicationName,
            string customisationName,
            string? supervisor,
            DateTime enrolled,
            DateTime lastUpdated,
            DateTime? completeBy,
            DateTime? completed,
            DateTime? evaluated,
            int enrollmentMethodId,
            int loginCount,
            int learningTime,
            int? diagnosticScore,
            bool isAssessed
        )
        {
            ApplicationName = applicationName;
            CustomisationName = customisationName;
            Supervisor = supervisor;
            Enrolled = enrolled;
            LastUpdated = lastUpdated;
            CompleteBy = completeBy;
            Completed = completed;
            Evaluated = evaluated;
            EnrollmentMethodId = enrollmentMethodId;
            LoginCount = loginCount;
            LearningTime = learningTime;
            DiagnosticScore = diagnosticScore;
            IsAssessed = isAssessed;
        }

        public string ApplicationName { get; set; }
        public string CustomisationName { get; set; }
        public string? Supervisor { get; set; }
        public DateTime Enrolled { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? CompleteBy { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? Evaluated { get; set; }
        public int EnrollmentMethodId { get; set; }
        public int LoginCount { get; set; }
        public int LearningTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public bool IsAssessed { get; set; }
    }
}
