namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using DigitalLearningSolutions.Web.Helpers;
    using HtmlAgilityPack;
    using Microsoft.Extensions.Configuration;

    public class TutorialViewModel
    {
        public string TutorialName { get; }
        public string SectionName { get; }
        public string CourseTitle { get; }
        public string Status { get; }
        public string? TutorialPath { get; }
        public string? VideoPath { get; }
        public string? Objectives { get; }
        public bool ShowLearnStatus { get; }
        public string SupportingMaterialsLabel { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }
        public int TutorialId { get; }
        public bool CanShowProgress { get; }
        public string TutorialRecommendation { get; }
        public string ScoreSummary { get; }
        public TutorialTimeSummaryViewModel TimeSummary { get; }
        public string? SupportingMaterialPath { get; }
        public TutorialNextLinkViewModel NextLinkViewModel { get; }
        public bool OnlyItemInOnlySection { get; }
        public bool OnlyItemInThisSection { get; }
        public bool ShowCompletionSummary { get; }
        public CompletionSummaryCardViewModel CompletionSummaryCardViewModel { get; }
        public string TutorialStartButtonAdditionalStyling { get; }
        public string TutorialStartButtonText { get; }

        public TutorialViewModel(
            IConfiguration config,
            TutorialInformation tutorialInformation,
            int customisationId,
            int sectionId
        )
        {
            TutorialName = tutorialInformation.Name;
            SectionName = tutorialInformation.SectionName;
            CourseTitle = tutorialInformation.CourseTitle;
            TutorialPath = tutorialInformation.TutorialPath;
            VideoPath = tutorialInformation.VideoPath;
            Status = tutorialInformation.Status;
            ShowLearnStatus = tutorialInformation.CourseSettings.ShowLearnStatus;
            SupportingMaterialsLabel =
                tutorialInformation.CourseSettings.SupportingInformation ?? "Download supporting materials";

            CustomisationId = customisationId;
            SectionId = sectionId;
            TutorialId = tutorialInformation.Id;
            Objectives = ParseObjectives(tutorialInformation.Objectives);

            CanShowProgress = GetCanShowProgress(
                tutorialInformation.CourseSettings.ShowLearnStatus,
                tutorialInformation.CanShowDiagnosticStatus,
                tutorialInformation.AttemptCount
            );
            TutorialRecommendation =
                GetTutorialRecommendation(tutorialInformation.CurrentScore, tutorialInformation.PossibleScore);
            ScoreSummary = GetScoreSummary(tutorialInformation.CurrentScore, tutorialInformation.PossibleScore);
            TimeSummary = new TutorialTimeSummaryViewModel(
                tutorialInformation.TimeSpent,
                tutorialInformation.AverageTutorialDuration,
                tutorialInformation.CourseSettings.ShowTime,
                tutorialInformation.CourseSettings.ShowLearnStatus
            );
            SupportingMaterialPath =
                ContentUrlHelper.GetNullableContentPath(config, tutorialInformation.SupportingMaterialPath);
            NextLinkViewModel = new TutorialNextLinkViewModel(
                customisationId,
                sectionId,
                tutorialInformation.PostLearningAssessmentPath,
                tutorialInformation.NextTutorialId,
                tutorialInformation.NextSectionId
            );

            OnlyItemInOnlySection = !tutorialInformation.OtherItemsInSectionExist && !tutorialInformation.OtherSectionsExist;
            OnlyItemInThisSection = !tutorialInformation.OtherItemsInSectionExist;
            ShowCompletionSummary = OnlyItemInOnlySection && tutorialInformation.IncludeCertification;

            CompletionSummaryCardViewModel = new CompletionSummaryCardViewModel(
                customisationId,
                tutorialInformation.Completed,
                tutorialInformation.MaxPostLearningAssessmentAttempts,
                tutorialInformation.IsAssessed,
                tutorialInformation.PostLearningAssessmentPassThreshold,
                tutorialInformation.DiagnosticAssessmentCompletionThreshold,
                tutorialInformation.TutorialsCompletionThreshold
            );
            TutorialStartButtonAdditionalStyling = tutorialInformation.Status == "Complete" ? "nhsuk-button--secondary" : "";
            TutorialStartButtonText = tutorialInformation.Status == "Complete" ? "Restart tutorial" : "Start tutorial";
        }

        private bool GetCanShowProgress(bool showLearnStatus, bool canShowDiagnosticStatus, int attemptCount)
        {
            return showLearnStatus && canShowDiagnosticStatus && attemptCount > 0;
        }

        private string GetTutorialRecommendation(int currentScore, int possibleScore)
        {
            return currentScore < possibleScore ? "recommended" : "optional";
        }

        private string GetScoreSummary(int currentScore, int possibleScore)
        {
            return CanShowProgress ? $"(score: {currentScore} out of {possibleScore})" : "";
        }

        private static string? ParseObjectives(string? objectives)
        {
            if (objectives == null)
            {
                return null;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(objectives);

            var body = doc.DocumentNode.SelectSingleNode("//body");

            return body?.InnerHtml ?? objectives;
        }
    }
}
