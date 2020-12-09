namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.TutorialContent;

    public class TutorialViewModel
    {
        public TutorialContent TutorialContent { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }

        public TutorialViewModel(TutorialContent tutorialContent, int customisationId, int sectionId)
        {
            TutorialContent = tutorialContent;
            CustomisationId = customisationId;
            SectionId = sectionId;
        }

        private static string ParseMinutes(int minutes)
        {
            if (minutes > 60)
            {
                return $"{minutes / 60}h {minutes % 60}m";
            }
            return $"{minutes}m";
        }

        public bool CanShowProgress => TutorialContent.CanShowDiagnosticStatus && TutorialContent.AttemptCount > 0;

        public string TutorialRecommendation =>
            TutorialContent.CurrentScore < TutorialContent.PossibleScore ? "recommended" : "optional";

        public string ScoreSummary =>
            CanShowProgress ? $"(score: {TutorialContent.CurrentScore} out of {TutorialContent.PossibleScore})" : "";

        public string GetTimeSummary =>
            $"{ParseMinutes(TutorialContent.TimeSpent)}" +
            $" (average time {ParseMinutes(TutorialContent.AverageTutorialDuration)})";
    }
}
