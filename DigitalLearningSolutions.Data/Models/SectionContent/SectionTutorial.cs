namespace DigitalLearningSolutions.Data.Models.SectionContent
{
    public class SectionTutorial
    {
        public string TutorialName { get; }
        public string CompletionStatus { get; }
        public int TutorialTime { get; }
        public int AverageTutorialTime { get; }
        public SectionTutorial(string tutorialName, string completionStatus, int tutorialTime, int averageTutorialTime)
        {
            TutorialName = tutorialName;
            CompletionStatus = completionStatus;
            TutorialTime = tutorialTime;
            AverageTutorialTime = averageTutorialTime;
        }
    }
}
