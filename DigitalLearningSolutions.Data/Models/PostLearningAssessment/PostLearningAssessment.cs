namespace DigitalLearningSolutions.Data.Models.PostLearningAssessment
{
    using System;

    public class PostLearningAssessment
    {
        public string CourseTitle { get; }
        public string? CourseDescription { get; }
        public string SectionName { get; }
        public int PostLearningScore { get; }
        public int PostLearningAttempts { get; }
        public bool PostLearningPassed { get; }
        public bool PostLearningLocked { get; }
        public bool IncludeCertification { get; }
        public bool IsAssessed { get; }
        public DateTime? Completed { get; }
        public int MaxPostLearningAssessmentAttempts { get; }
        public int PostLearningAssessmentPassThreshold { get; }
        public int DiagnosticAssessmentCompletionThreshold { get; }
        public int TutorialsCompletionThreshold { get; }
        public int? NextSectionId { get; }
        public bool OtherSectionsExist { get; }
        public bool OtherItemsInSectionExist { get; }
        public string? Password { get; }
        public bool PasswordSubmitted { get; }

        public PostLearningAssessment(
            string applicationName,
            string? applicationInfo,
            string customisationName,
            string sectionName,
            int bestScore,
            int attemptsPl,
            int plPasses,
            bool plLocked,
            bool includeCertification,
            bool isAssessed,
            DateTime? completed,
            int maxPostLearningAssessmentAttempts,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold,
            int? nextSectionId,
            bool otherSectionsExist,
            bool otherItemsInSectionExist,
            string? password,
            bool passwordSubmitted
        )
        {
            CourseTitle = $"{applicationName} - {customisationName}";
            CourseDescription = applicationInfo;
            SectionName = sectionName;
            PostLearningScore = bestScore;
            PostLearningAttempts = attemptsPl;
            PostLearningPassed = plPasses > 0;
            PostLearningLocked = plLocked;
            IncludeCertification = includeCertification;
            IsAssessed = isAssessed;
            Completed = completed;
            MaxPostLearningAssessmentAttempts = maxPostLearningAssessmentAttempts;
            PostLearningAssessmentPassThreshold = postLearningAssessmentPassThreshold;
            DiagnosticAssessmentCompletionThreshold = diagnosticAssessmentCompletionThreshold;
            TutorialsCompletionThreshold = tutorialsCompletionThreshold;
            NextSectionId = nextSectionId;
            OtherSectionsExist = otherSectionsExist;
            OtherItemsInSectionExist = otherItemsInSectionExist;
            Password = password;
            PasswordSubmitted = passwordSubmitted;
        }
    }
}
