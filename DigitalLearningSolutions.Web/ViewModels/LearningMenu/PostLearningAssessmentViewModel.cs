namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.PostLearningAssessment;

    public class PostLearningAssessmentViewModel
    {
        public string CourseTitle { get; }
        public string? CourseDescription { get; }
        public string SectionName { get; }
        public string AssessmentStatus { get; }
        public string AssessmentStatusStyling { get; }
        public string StartButtonText { get; }
        public string StartButtonAdditionalStyling { get; }
        public string? ScoreInformation { get; }
        public bool PostLearningLocked { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }
        public int? NextSectionId { get; }
        public bool OnlyItemInOnlySection { get; }
        public bool OnlyItemInThisSection { get; }
        public bool ShowCompletionSummary { get; }
        public CompletionSummaryCardViewModel CompletionSummaryCardViewModel { get; }
        public bool ShowNextButton { get; }

        public PostLearningAssessmentViewModel(PostLearningAssessment postLearningAssessment, int customisationId, int sectionId)
        {
            CourseTitle = postLearningAssessment.CourseTitle;
            CourseDescription = postLearningAssessment.CourseDescription;
            SectionName = postLearningAssessment.SectionName;
            PostLearningLocked = postLearningAssessment.PostLearningLocked;
            CustomisationId = customisationId;
            SectionId = sectionId;
            NextSectionId = postLearningAssessment.NextSectionId;

            if (postLearningAssessment.PostLearningAttempts == 0)
            {
                AssessmentStatus = "Not attempted";
            }
            else
            {
                AssessmentStatus = GetPassStatus(postLearningAssessment);
                ScoreInformation = GetScoreInformation(postLearningAssessment);
            }
            AssessmentStatusStyling = GetPassStatusStyling(postLearningAssessment);

            StartButtonText = postLearningAssessment.PostLearningAttempts == 0
                ? "Start assessment"
                : "Restart assessment";
            StartButtonAdditionalStyling = postLearningAssessment.PostLearningAttempts == 0
                ? ""
                : "nhsuk-button--secondary";


            OnlyItemInOnlySection = !postLearningAssessment.OtherItemsInSectionExist && !postLearningAssessment.OtherSectionsExist;
            OnlyItemInThisSection = !postLearningAssessment.OtherItemsInSectionExist;
            ShowCompletionSummary = OnlyItemInOnlySection && postLearningAssessment.IncludeCertification;

            CompletionSummaryCardViewModel = new CompletionSummaryCardViewModel(
                customisationId,
                postLearningAssessment.Completed,
                postLearningAssessment.MaxPostLearningAssessmentAttempts,
                postLearningAssessment.IsAssessed,
                postLearningAssessment.PostLearningAssessmentPassThreshold,
                postLearningAssessment.DiagnosticAssessmentCompletionThreshold,
                postLearningAssessment.TutorialsCompletionThreshold
            );

            ShowNextButton = postLearningAssessment.PostLearningAttempts > 0 && !OnlyItemInOnlySection;
        }

        private string GetScoreInformation(PostLearningAssessment postLearningAssessment)
        {
            return postLearningAssessment.PostLearningAttempts == 1
                ? $"{postLearningAssessment.PostLearningScore}% - 1 attempt"
                : $"{postLearningAssessment.PostLearningScore}% - {postLearningAssessment.PostLearningAttempts} attempts";
        }

        private string GetPassStatus(PostLearningAssessment postLearningAssessment)
        {
            return postLearningAssessment.PostLearningPassed
               ? "Passed"
               : "Failed";
        }

        private string GetPassStatusStyling(PostLearningAssessment postLearningAssessment)
        {
            return postLearningAssessment.PostLearningAttempts > 0 && postLearningAssessment.PostLearningPassed
               ? "passed-text"
               : "not-passed-text";
        }
    }
}
