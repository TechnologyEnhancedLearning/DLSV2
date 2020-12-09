namespace DigitalLearningSolutions.Data.Models.TutorialContent
{
    public class TutorialContent
    {
        public int Id { get; }
        public string Name { get; }
        public string CourseTitle { get; }
        public string Status { get; }
        public int TimeSpent { get; }
        public int AverageTutorialDuration { get; }
        public int CurrentScore { get; }
        public int PossibleScore { get; }
        public bool CanShowDiagnosticStatus { get; }
        public int AttemptCount { get; }
        public string? Objectives { get; }
        public string? VideoPath { get; }
        public string? TutorialPath { get; }
        public string? SupportingMaterialPath { get; }

        public TutorialContent(
            int id,
            string name,
            string applicationName,
            string customisationName,
            string status,
            int timeSpent,
            int averageTutorialDuration,
            int currentScore,
            int possibleScore,
            bool canShowDiagnosticStatus,
            int attemptCount,
            string? objectives,
            string? videoPath,
            string? tutorialPath,
            string? supportingMaterialPath
        )
        {
            Id = id;
            Name = name;
            CourseTitle = $"{applicationName} - {customisationName}";
            Status = status;
            TimeSpent = timeSpent;
            AverageTutorialDuration = averageTutorialDuration;
            CurrentScore = currentScore;
            PossibleScore = possibleScore;
            CanShowDiagnosticStatus = canShowDiagnosticStatus;
            AttemptCount = attemptCount;
            Objectives = objectives;
            VideoPath = videoPath;
            TutorialPath = tutorialPath;
            SupportingMaterialPath = supportingMaterialPath;
        }
    }
}
