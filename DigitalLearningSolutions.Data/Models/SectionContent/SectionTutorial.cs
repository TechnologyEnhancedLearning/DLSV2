namespace DigitalLearningSolutions.Data.Models.SectionContent
{
    public class SectionTutorial
    {
        public string TutorialName { get; }
        public int TutorialStatus { get; }
        public string CompletionStatus { get; }
        public int TutorialTime { get; }
        public int AverageTutorialTime { get; }
        public int Id { get; }
        public bool CustomisationTutorialStatus { get; }
        public int CurrentScore { get; }
        public int PossibleScore { get; }
        public bool TutorialDiagnosticStatus { get; }
        public int TutorialDiagnosticAttempts { get; }

        public SectionTutorial(
            string tutorialName,
            int tutStat,
            string completionStatus,
            int tutTime,
            int averageTutMins,
            int id,
            bool status,
            int currentScore,
            int possibleScore,
            bool tutorialDiagnosticStatus,
            int tutorialDiagnosticAttempts
        )
        {
            TutorialName = tutorialName;
            TutorialStatus = tutStat;
            CompletionStatus = completionStatus;
            TutorialTime = tutTime;
            AverageTutorialTime = averageTutMins;
            Id = id;
            CustomisationTutorialStatus = status;
            CurrentScore = currentScore;
            PossibleScore = possibleScore;
            TutorialDiagnosticStatus = tutorialDiagnosticStatus;
            TutorialDiagnosticAttempts = tutorialDiagnosticAttempts;
        }
    }
}
