namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public class TutorialCardViewModel
    {
        public int Id { get; }
        public string TutorialName { get; }
        public string CompletionStatus { get; }
        public int SectionId { get; }
        public int CustomisationId { get; }
        public bool ShowLearnStatus { get; }
        public TutorialTimeSummaryViewModel TimeSummary { get; }
        public string RecommendationStatus { get; }
        public string StatusTagColour { get; }
        public bool ShowRecommendationStatus { get; }

        public TutorialCardViewModel(
            SectionTutorial tutorial,
            bool showTime,
            bool showLearnStatus,
            int sectionId,
            int customisationId,
            bool diagnosticStatus,
            int diagnosticAttempts
        )
        {
            Id = tutorial.Id;
            TutorialName = tutorial.TutorialName;
            CompletionStatus = tutorial.CompletionStatus;
            ShowLearnStatus = showLearnStatus;
            TimeSummary = new TutorialTimeSummaryViewModel(
                tutorial.TutorialTime,
                tutorial.AverageTutorialTime,
                showTime,
                showLearnStatus
            );
            SectionId = sectionId;
            CustomisationId = customisationId;
            RecommendationStatus = tutorial.CurrentScore < tutorial.PossibleScore ? "Recommended" : "Optional";
            StatusTagColour = tutorial.CurrentScore < tutorial.PossibleScore ? "nhsuk-tag--orange" : "nhsuk-tag--green";
            ShowRecommendationStatus = diagnosticAttempts > 0 && showLearnStatus && diagnosticStatus;
        }
    }
}
