namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public class TutorialCardViewModel
    {
        public int Id { get; }
        public string TutorialName { get; }
        public string CompletionStatus { get; }
        public string TimeSpentInformation { get; }
        public string AverageTimeInformation { get; }
        public int SectionId { get; }
        public int CustomisationId { get; }
        public bool ShowTime { get; }
        public bool ShowLearnStatus { get; }

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
            TimeSpentInformation = tutorial.TutorialTime == 1
                ? $"{tutorial.TutorialTime} minute spent"
                : $"{tutorial.TutorialTime} minutes spent";
            AverageTimeInformation = tutorial.AverageTutorialTime == 1
                ? $"(average tutorial time {tutorial.AverageTutorialTime} minute)"
                : $"(average tutorial time {tutorial.AverageTutorialTime} minutes)";
            ShowTime = showTime && showLearnStatus;
            ShowLearnStatus = showLearnStatus;
            SectionId = sectionId;
            CustomisationId = customisationId;
        }
    }
}
