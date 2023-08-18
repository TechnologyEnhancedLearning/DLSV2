namespace DigitalLearningSolutions.Data.Models.TutorialContent
{
    using System;

    public class TutorialInformation
    {
        public int Id { get; }
        public string Name { get; }
        public string SectionName { get; }
        public string CourseTitle { get; }
        public string? CourseDescription { get; }
        public string Status { get; }
        public int TimeSpent { get; }
        public int AverageTutorialDuration { get; }
        public int CurrentScore { get; }
        public int PossibleScore { get; }
        public bool CanShowDiagnosticStatus { get; }
        public int AttemptCount { get; }
        public string? Objectives { get; }
        public string? VideoPath { get; }
        public string? TutorialPath { get; }
        public string? SupportingMaterialPath { get; }
        public string? PostLearningAssessmentPath { get; }
        public CourseSettings CourseSettings { get; }
        public bool IncludeCertification { get; }
        public bool IsAssessed { get; }
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

        public TutorialInformation(
            int id,
            string name,
            string sectionName,
            string applicationName,
            string? applicationInfo,
            string customisationName,
            string status,
            int timeSpent,
            int averageTutorialDuration,
            int currentScore,
            int possibleScore,
            bool canShowDiagnosticStatus,
            int attemptCount,
            string? objectives,
            string? videoPath,
            string? tutorialPath,
            string? supportingMaterialPath,
            string? postLearningAssessmentPath,
            string? courseSettings,
            bool includeCertification,
            bool isAssessed,
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
            Id = id;
            Name = name;
            SectionName = sectionName;
            CourseTitle = !String.IsNullOrEmpty(customisationName) ? $"{applicationName} - {customisationName}" : applicationName;
            CourseDescription = applicationInfo;
            Status = status;
            TimeSpent = timeSpent;
            AverageTutorialDuration = averageTutorialDuration;
            CurrentScore = currentScore;
            PossibleScore = possibleScore;
            CanShowDiagnosticStatus = canShowDiagnosticStatus;
            AttemptCount = attemptCount;
            Objectives = objectives;
            VideoPath = videoPath;
            TutorialPath = tutorialPath;
            SupportingMaterialPath = supportingMaterialPath;
            PostLearningAssessmentPath = postLearningAssessmentPath;
            CourseSettings = new CourseSettings(courseSettings);
            IncludeCertification = includeCertification;
            IsAssessed = isAssessed;
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
