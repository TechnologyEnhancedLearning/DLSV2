namespace DigitalLearningSolutions.Data.Models.SectionContent
{
    using System;
    using System.Collections.Generic;

    public class SectionContent
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public bool HasLearning { get; }
        public int DiagnosticAttempts { get; set; }
        public int SectionScore { get; set; }
        public int MaxSectionScore { get; set; }
        public string? DiagnosticAssessmentPath { get; }
        public string? PostLearningAssessmentPath { get; }
        public int PostLearningAttempts { get; }
        public bool PostLearningPassed { get; }
        public bool DiagnosticStatus { get; set; }
        public bool IsAssessed { get; }
        public string? ConsolidationPath { get; }
        public CourseSettings CourseSettings { get; }
        public bool IncludeCertification { get; }
        public DateTime? Completed { get; }
        public int MaxPostLearningAssessmentAttempts { get; }
        public int PostLearningAssessmentPassThreshold { get; }
        public int DiagnosticAssessmentCompletionThreshold { get; }
        public int TutorialsCompletionThreshold { get; }
        public bool OtherSectionsExist { get; }
        public int? NextSectionId { get; }

        public List<SectionTutorial> Tutorials { get; } = new List<SectionTutorial>();

        public SectionContent(
            string applicationName,
            string customisationName,
            string sectionName,
            bool hasLearning,
            int diagAttempts,
            int diagLast,
            int diagAssessOutOf,
            string? diagAssessPath,
            string? plAssessPath,
            int attemptsPl,
            int plPasses,
            bool diagStatus,
            bool isAssessed,
            string? consolidationPath,
            string? courseSettings,
            bool includeCertification,
            DateTime? completed,
            int maxPostLearningAssessmentAttempts,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold,
            bool otherSectionsExist,
            int? nextSectionId
        )
        {
            CourseTitle = $"{applicationName} - {customisationName}";
            SectionName = sectionName;
            HasLearning = hasLearning;
            DiagnosticAttempts = diagAttempts;
            SectionScore = diagLast;
            MaxSectionScore = diagAssessOutOf;
            DiagnosticAssessmentPath = diagAssessPath;
            PostLearningAssessmentPath = plAssessPath;
            PostLearningAttempts = attemptsPl;
            PostLearningPassed = plPasses > 0;
            DiagnosticStatus = diagStatus;
            IsAssessed = isAssessed;
            ConsolidationPath = consolidationPath;
            CourseSettings = new CourseSettings(courseSettings);
            IncludeCertification = includeCertification;
            Completed = completed;
            MaxPostLearningAssessmentAttempts = maxPostLearningAssessmentAttempts;
            PostLearningAssessmentPassThreshold = postLearningAssessmentPassThreshold;
            DiagnosticAssessmentCompletionThreshold = diagnosticAssessmentCompletionThreshold;
            TutorialsCompletionThreshold = tutorialsCompletionThreshold;
            OtherSectionsExist = otherSectionsExist;
            NextSectionId = nextSectionId;
        }
    }
}
