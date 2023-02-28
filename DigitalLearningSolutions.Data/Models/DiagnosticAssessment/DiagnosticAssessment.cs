namespace DigitalLearningSolutions.Data.Models.DiagnosticAssessment
{
    using System;
    using System.Collections.Generic;

    public class DiagnosticAssessment
    {
        public string CourseTitle { get; }
        public string? CourseDescription { get; }
        public string SectionName { get; }
        public int DiagnosticAttempts { get; set; }
        public int SectionScore { get; set; }
        public int MaxSectionScore { get; set; }
        public string DiagnosticAssessmentPath { get; }
        public bool CanSelectTutorials { get; }
        public string? PostLearningAssessmentPath { get; }
        public bool IsAssessed { get; }
        public bool IncludeCertification { get; }
        public DateTime? Completed { get; }
        public int MaxPostLearningAssessmentAttempts { get; }
        public int PostLearningAssessmentPassThreshold { get; }
        public int DiagnosticAssessmentCompletionThreshold { get; }
        public int TutorialsCompletionThreshold { get; }
        public int? NextTutorialId { get; }
        public int? NextSectionId { get; }
        public bool OtherSectionsExist { get; }
        public bool OtherItemsInSectionExist { get; }
        public string? Password { get; }
        public bool PasswordSubmitted { get; }
        public List<DiagnosticTutorial> Tutorials { get; } = new List<DiagnosticTutorial>();

        public DiagnosticAssessment(
            string applicationName,
            string? applicationInfo,
            string customisationName,
            string sectionName,
            int diagAttempts,
            int diagLast,
            int diagAssessOutOf,
            string diagAssessPath,
            bool diagObjSelect,
            string? plAssessPath,
            bool isAssessed,
            bool includeCertification,
            DateTime? completed,
            int maxPostLearningAssessmentAttempts,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold,
            int? nextTutorialId,
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
            DiagnosticAttempts = diagAttempts;
            SectionScore = diagLast;
            MaxSectionScore = diagAssessOutOf;
            DiagnosticAssessmentPath = diagAssessPath;
            CanSelectTutorials = diagObjSelect;
            PostLearningAssessmentPath = plAssessPath;
            IsAssessed = isAssessed;
            IncludeCertification = includeCertification;
            Completed = completed;
            MaxPostLearningAssessmentAttempts = maxPostLearningAssessmentAttempts;
            PostLearningAssessmentPassThreshold = postLearningAssessmentPassThreshold;
            DiagnosticAssessmentCompletionThreshold = diagnosticAssessmentCompletionThreshold;
            TutorialsCompletionThreshold = tutorialsCompletionThreshold;
            NextTutorialId = nextTutorialId;
            NextSectionId = nextSectionId;
            OtherSectionsExist = otherSectionsExist;
            OtherItemsInSectionExist = otherItemsInSectionExist;
            Password = password;
            PasswordSubmitted = passwordSubmitted;
        }
    }
}
