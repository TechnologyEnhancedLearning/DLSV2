namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using DigitalLearningSolutions.Web.Helpers;
    using HtmlAgilityPack;
    using Microsoft.Extensions.Configuration;

    public class TutorialViewModel
    {
        public string TutorialName { get; }
        public string CourseTitle { get; }
        public string Status { get; }
        public string? TutorialPath { get; }
        public string? VideoPath { get; }
        public string? Objectives { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }
        public int TutorialId { get; }
        public bool CanShowProgress { get; }
        public string TutorialRecommendation { get; }
        public string ScoreSummary { get; }
        public string TimeSummary { get; }
        public string? SupportingMaterialPath { get; }

        public TutorialViewModel(
            IConfiguration config,
            TutorialInformation tutorialInformation,
            int customisationId,
            int sectionId
        )
        {
            TutorialName = tutorialInformation.Name;
            CourseTitle = tutorialInformation.CourseTitle;
            TutorialPath = tutorialInformation.TutorialPath;
            VideoPath = tutorialInformation.VideoPath;
            Status = tutorialInformation.Status;

            CustomisationId = customisationId;
            SectionId = sectionId;
            TutorialId = tutorialInformation.Id;
            Objectives = ParseObjectives(tutorialInformation.Objectives);

            CanShowProgress =
                GetCanShowProgress(tutorialInformation.CanShowDiagnosticStatus, tutorialInformation.AttemptCount);
            TutorialRecommendation =
                GetTutorialRecommendation(tutorialInformation.CurrentScore, tutorialInformation.PossibleScore);
            ScoreSummary = GetScoreSummary(tutorialInformation.CurrentScore, tutorialInformation.PossibleScore);
            TimeSummary = GetTimeSummary(tutorialInformation.TimeSpent, tutorialInformation.AverageTutorialDuration);
            SupportingMaterialPath =
                ContentUrlHelper.GetNullableContentPath(config, tutorialInformation.SupportingMaterialPath);
        }

        private bool GetCanShowProgress(bool canShowDiagnosticStatus, int attemptCount)
        {
            return canShowDiagnosticStatus && attemptCount > 0;
        }

        private string GetTutorialRecommendation(int currentScore, int possibleScore)
        {
            return currentScore < possibleScore ? "recommended" : "optional";
        }

        private string GetScoreSummary(int currentScore, int possibleScore)
        {
            return CanShowProgress ? $"(score: {currentScore} out of {possibleScore})" : "";
        }

        private string GetTimeSummary(int timeSpent, int averageTutorialDuration)
        {
            return $"{ParseMinutes(timeSpent)}" +
                   $" (average time {ParseMinutes(averageTutorialDuration)})";
        }

        private static string ParseMinutes(int minutes)
        {
            if (minutes > 60)
            {
                return $"{minutes / 60}h {minutes % 60}m";
            }
            return $"{minutes}m";
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
