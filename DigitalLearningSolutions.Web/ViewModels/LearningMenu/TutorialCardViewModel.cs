namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public class TutorialCardViewModel
    {
        public int Id { get; }
        public string TutorialName { get; }
        public string CompletionStatus { get; }
        public string TimeInformation { get; }
        public int SectionId { get; }
        public int CustomisationId { get; }

        public TutorialCardViewModel(SectionTutorial tutorial, int sectionId, int customisationId)
        {
            Id = tutorial.Id;
            TutorialName = tutorial.TutorialName;
            CompletionStatus = tutorial.CompletionStatus;
            TimeInformation = $"{tutorial.TutorialTime}m (average time {tutorial.AverageTutorialTime}m)";
            SectionId = sectionId;
            CustomisationId = customisationId;
        }
    }
}
