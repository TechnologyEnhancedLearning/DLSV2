namespace DigitalLearningSolutions.Data.Models.CourseContent
{
    using System;
    using System.Collections.Generic;

    public class CourseContent
    {
        public int Id { get; }
        public string Title { get; }
        public string AverageDuration { get; }
        public string CentreName { get; }
        public string? BannerText { get; }
        public bool IncludeCertification { get; }
        public DateTime? Completed { get; }
        public int AssessAttempts { get; }
        public bool IsAssessed { get; }
        public int PostLearningAssessmentPassThreshold { get; }
        public int DiagnosticAssessmentCompletionThreshold { get; }
        public int TutorialsCompletionThreshold { get; }
        public List<CourseSection> Sections { get; } = new List<CourseSection>();

        public CourseContent(
            int id,
            string applicationName,
            string customisationName,
            string averageDuration,
            string centreName,
            string? bannerText,
            bool includeCertification,
            DateTime? completed,
            int assessAttempts,
            bool isAssessed,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold
        )
        {
            Id = id;
            Title = $"{applicationName} - {customisationName}";
            AverageDuration = averageDuration;
            CentreName = centreName;
            BannerText = bannerText;
            IncludeCertification = includeCertification;
            Completed = completed;
            AssessAttempts = assessAttempts;
            IsAssessed = isAssessed;
            PostLearningAssessmentPassThreshold = postLearningAssessmentPassThreshold;
            DiagnosticAssessmentCompletionThreshold = diagnosticAssessmentCompletionThreshold;
            TutorialsCompletionThreshold = tutorialsCompletionThreshold;
        }
    }
}
