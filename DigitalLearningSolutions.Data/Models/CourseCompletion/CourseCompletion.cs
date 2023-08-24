namespace DigitalLearningSolutions.Data.Models.CourseCompletion
{
    using System;

    public class CourseCompletion
    {
        public int Id { get; }
        public string CourseTitle { get; }
        public DateTime? Completed { get; }
        public DateTime? Evaluated { get; }
        public int MaxPostLearningAssessmentAttempts { get; }
        public bool IsAssessed { get; }
        public int PostLearningAssessmentPassThreshold { get; }
        public int DiagnosticAssessmentCompletionThreshold { get; }
        public int TutorialsCompletionThreshold { get; }
        public int? DiagnosticScore { get; }
        public int DiagnosticAttempts { get; }
        public double PercentageTutorialsCompleted { get; }
        public int PostLearningPasses { get; }
        public int SectionCount { get; }

        public CourseCompletion(
            int id,
            string applicationName,
            string customisationName,
            DateTime? completed,
            DateTime? evaluated,
            int maxPostLearningAssessmentAttempts,
            bool isAssessed,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold,
            int? diagnosticScore,
            int diagnosticAttempts,
            double percentageTutorialsCompleted,
            int postLearningPasses,
            int sectionCount
        )
        {
            Id = id;
            CourseTitle = !String.IsNullOrEmpty(customisationName) ? $"{applicationName} - {customisationName}" : applicationName;
            Completed = completed;
            Evaluated = evaluated;
            MaxPostLearningAssessmentAttempts = maxPostLearningAssessmentAttempts;
            IsAssessed = isAssessed;
            PostLearningAssessmentPassThreshold = postLearningAssessmentPassThreshold;
            DiagnosticAssessmentCompletionThreshold = diagnosticAssessmentCompletionThreshold;
            TutorialsCompletionThreshold = tutorialsCompletionThreshold;
            DiagnosticScore = diagnosticScore;
            DiagnosticAttempts = diagnosticAttempts;
            PercentageTutorialsCompleted = percentageTutorialsCompleted;
            PostLearningPasses = postLearningPasses;
            SectionCount = sectionCount;
        }
    }
}
