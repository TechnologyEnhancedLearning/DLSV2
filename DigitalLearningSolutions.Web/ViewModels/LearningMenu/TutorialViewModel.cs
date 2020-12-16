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
        public string? SupportingMaterialPath { get; }

        private readonly int timeSpent;
        private readonly int averageTutorialDuration;
        private readonly int currentScore;
        private readonly int possibleScore;
        private readonly bool canShowDiagnosticStatus;
        private readonly int attemptCount;

        public TutorialViewModel(
            IConfiguration config,
            TutorialInformation tutorialInformation,
            int customisationId,
            int sectionId,
            int tutorialId
        )
        {
            TutorialName = tutorialInformation.Name;
            CourseTitle = tutorialInformation.CourseTitle;
            TutorialPath = tutorialInformation.TutorialPath;
            VideoPath = tutorialInformation.VideoPath;
            Status = tutorialInformation.Status;

            timeSpent = tutorialInformation.TimeSpent;
            averageTutorialDuration = tutorialInformation.AverageTutorialDuration;
            currentScore = tutorialInformation.CurrentScore;
            possibleScore = tutorialInformation.PossibleScore;
            canShowDiagnosticStatus = tutorialInformation.CanShowDiagnosticStatus;
            attemptCount = tutorialInformation.AttemptCount;

            CustomisationId = customisationId;
            SectionId = sectionId;
            TutorialId = tutorialId;
            Objectives = ParseObjectives(tutorialInformation.Objectives);

            SupportingMaterialPath =
                ContentUrlHelper.GetNullableContentPath(config, tutorialInformation.SupportingMaterialPath);
        }

        public bool CanShowProgress => canShowDiagnosticStatus && attemptCount > 0;

        public string TutorialRecommendation =>
            currentScore < possibleScore ? "recommended" : "optional";

        public string ScoreSummary =>
            CanShowProgress ? $"(score: {currentScore} out of {possibleScore})" : "";

        public string TimeSummary =>
            $"{ParseMinutes(timeSpent)}" +
            $" (average time {ParseMinutes(averageTutorialDuration)})";

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
