namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public class TutorialCardViewModel
    {
        public int Id { get; }
        public string TutorialName { get; }
        public string CompletionStatus { get; }
        public int TutorialTime { get; }
        public int AverageTutorialTime { get; }
        public int SectionId { get; }
        public int CustomisationId { get; }

        public TutorialCardViewModel(SectionTutorial tutorial, int sectionId, int customisationId)
        {
            Id = tutorial.Id;
            TutorialName = tutorial.TutorialName;
            CompletionStatus = tutorial.CompletionStatus;
            TutorialTime = tutorial.TutorialTime;
            AverageTutorialTime = tutorial.AverageTutorialTime;
            SectionId = sectionId;
            CustomisationId = customisationId;
        }
    }
}
