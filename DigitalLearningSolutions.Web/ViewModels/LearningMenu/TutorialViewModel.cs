namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Linq;
    using System.Net;
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using HtmlAgilityPack;

    public class TutorialViewModel
    {
        public TutorialInformation TutorialInformation { get; }
        public string? Objectives { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }

        public TutorialViewModel(TutorialInformation tutorialInformation, int customisationId, int sectionId)
        {
            TutorialInformation = tutorialInformation;
            CustomisationId = customisationId;
            SectionId = sectionId;
            Objectives = ParseObjectives(tutorialInformation.Objectives);
        }

        public bool CanShowProgress => TutorialInformation.CanShowDiagnosticStatus && TutorialInformation.AttemptCount > 0;

        public string TutorialRecommendation =>
            TutorialInformation.CurrentScore < TutorialInformation.PossibleScore ? "recommended" : "optional";

        public string ScoreSummary =>
            CanShowProgress ? $"(score: {TutorialInformation.CurrentScore} out of {TutorialInformation.PossibleScore})" : "";

        public string TimeSummary =>
            $"{ParseMinutes(TutorialInformation.TimeSpent)}" +
            $" (average time {ParseMinutes(TutorialInformation.AverageTutorialDuration)})";

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

            var body = doc.DocumentNode.SelectNodes("//body");

            return body?.FirstOrDefault()?.InnerHtml
                   ?? WebUtility.HtmlDecode(objectives);
        }
    }
}
