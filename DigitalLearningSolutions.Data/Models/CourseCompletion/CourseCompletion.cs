namespace DigitalLearningSolutions.Data.Models.CourseCompletion
{
    using System;

    public class CourseCompletion
    {
        public int Id { get; }
        public string ApplicationName { get; }
        public string CustomisationName { get; }
        public bool IncludeCertification { get; }
        public DateTime? Completed { get; }
        public DateTime? Evaluated { get; }
        public int MaxPostLearningAssessmentAttempts { get; }
        public bool IsAssessed { get; }
        public int PostLearningAssessmentPassThreshold { get; }
        public int DiagnosticAssessmentCompletionThreshold { get; }
        public int TutorialsCompletionThreshold { get; }
        public int? DiagnosticScore { get; }
        public int DiagnosticAttempts { get; }
        public int LearningDone { get; }
        public int PostLearningPasses { get; }
        public int SectionCount { get; }

        public CourseCompletion(
            int id,
            string applicationName,
            string customisationName,
            bool includeCertification,
            DateTime? completed,
            DateTime? evaluated,
            int maxPostLearningAssessmentAttempts,
            bool isAssessed,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold,
            int? diagnosticScore,
            int diagnosticAttempts,
            int learningDone,
            int postLearningPasses,
            int sectionCount
        )
        {
            Id = id;
            ApplicationName = applicationName;
            CustomisationName = customisationName;
            IncludeCertification = includeCertification;
            Completed = completed;
            Evaluated = evaluated;
            MaxPostLearningAssessmentAttempts = maxPostLearningAssessmentAttempts;
            IsAssessed = isAssessed;
            PostLearningAssessmentPassThreshold = postLearningAssessmentPassThreshold;
            DiagnosticAssessmentCompletionThreshold = diagnosticAssessmentCompletionThreshold;
            TutorialsCompletionThreshold = tutorialsCompletionThreshold;
            DiagnosticScore = diagnosticScore;
            DiagnosticAttempts = diagnosticAttempts;
            LearningDone = learningDone;
            PostLearningPasses = postLearningPasses;
            SectionCount = sectionCount;
        }
    }
}
