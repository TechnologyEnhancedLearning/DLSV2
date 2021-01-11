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

        public TutorialCardViewModel(
            SectionTutorial tutorial,
            bool showTime,
            bool showLearnStatus,
            int sectionId,
            int customisationId
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
        }
    }
}
