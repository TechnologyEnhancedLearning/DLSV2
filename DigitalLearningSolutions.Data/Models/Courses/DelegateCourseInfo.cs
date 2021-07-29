namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public class DelegateCourseInfo
    {
        public DelegateCourseInfo() { }

        public DelegateCourseInfo(
            int customisationId,
            string applicationName,
            string customisationName,
            string? supervisorForename,
            string? supervisorSurname,
            DateTime enrolled,
            DateTime lastUpdated,
            DateTime? completeBy,
            DateTime? completed,
            DateTime? evaluated,
            int enrolmentMethodId,
            int loginCount,
            int learningTime,
            int? diagnosticScore,
            bool isAssessed,
            string? answer1,
            string? answer2,
            string? answer3
        )
        {
            CustomisationId = customisationId;
            ApplicationName = applicationName;
            CustomisationName = customisationName;
            SupervisorForename = supervisorForename;
            SupervisorSurname = supervisorSurname;
            Enrolled = enrolled;
            LastUpdated = lastUpdated;
            CompleteBy = completeBy;
            Completed = completed;
            Evaluated = evaluated;
            EnrolmentMethodId = enrolmentMethodId;
            LoginCount = loginCount;
            LearningTime = learningTime;
            DiagnosticScore = diagnosticScore;
            IsAssessed = isAssessed;
            Answer1 = answer1;
            Answer2 = answer2;
            Answer3 = answer3;
        }

        public int CustomisationId { get; set; }
        public string ApplicationName { get; set; }
        public string CustomisationName { get; set; }
        public string? SupervisorForename { get; set; }
        public string? SupervisorSurname { get; set; }
        public DateTime Enrolled { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? CompleteBy { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? Evaluated { get; set; }
        public int EnrolmentMethodId { get; set; }
        public int LoginCount { get; set; }
        public int LearningTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public bool IsAssessed { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
    }
}
