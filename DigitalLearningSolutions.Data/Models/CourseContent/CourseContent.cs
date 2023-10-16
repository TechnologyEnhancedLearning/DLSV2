namespace DigitalLearningSolutions.Data.Models.CourseContent
{
    using System;
    using System.Collections.Generic;

    public class CourseContent
    {
        public int Id { get; }
        public string Title { get; }
        public string? Description { get; }
        public int? AverageDuration { get; }
        public string CentreName { get; }
        public string? BannerText { get; }
        public bool IncludeCertification { get; }
        public DateTime? Completed { get; }
        public int MaxPostLearningAssessmentAttempts { get; }
        public bool IsAssessed { get; }
        public int PostLearningAssessmentPassThreshold { get; }
        public int DiagnosticAssessmentCompletionThreshold { get; }
        public int TutorialsCompletionThreshold { get; }
        public CourseSettings CourseSettings { get; }
        public string? Password { get; }
        public bool PasswordSubmitted { get; }
        public List<CourseSection> Sections { get; } = new List<CourseSection>();

        public CourseContent(
            int id,
            string applicationName,
            string? applicationInfo,
            string customisationName,
            int? averageDuration,
            string centreName,
            string? bannerText,
            bool includeCertification,
            DateTime? completed,
            int maxPostLearningAssessmentAttempts,
            bool isAssessed,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold,
            string? courseSettings,
            string? password,
            bool passwordSubmitted
        )
        {
            Id = id;
            Title = !String.IsNullOrEmpty(customisationName) ? $"{applicationName} - {customisationName}" : applicationName;
            Description = applicationInfo;
            AverageDuration = averageDuration;
            CentreName = centreName;
            BannerText = bannerText;
            IncludeCertification = includeCertification;
            Completed = completed;
            MaxPostLearningAssessmentAttempts = maxPostLearningAssessmentAttempts;
            IsAssessed = isAssessed;
            PostLearningAssessmentPassThreshold = postLearningAssessmentPassThreshold;
            DiagnosticAssessmentCompletionThreshold = diagnosticAssessmentCompletionThreshold;
            TutorialsCompletionThreshold = tutorialsCompletionThreshold;
            CourseSettings = new CourseSettings(courseSettings);
            Password = password;
            PasswordSubmitted = passwordSubmitted;
        }
    }
}
