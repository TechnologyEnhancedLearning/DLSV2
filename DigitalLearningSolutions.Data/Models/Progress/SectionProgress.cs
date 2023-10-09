namespace DigitalLearningSolutions.Data.Models.Progress
{
    public class SectionProgress
    {
        public int SectionID { get; set; }
        public int ApplicationID { get; set; }
        public int SectionNumber { get; set; }
        public string? SectionName { get; set; }
        public int PCComplete { get; set; }
        public int TimeMins { get; set; }
        public int DiagAttempts { get; set; }
        public int SecScore { get; set; }
        public int SecOutOf { get; set; }
        public string? ConsolidationPath { get; set; }
        public int AvgSecTime { get; set; }
        public string? DiagAssessPath { get; set; }
        public string? PLAssessPath { get; set; }
        public bool LearnStatus { get; set; }
        public bool DiagStatus { get; set; }
        public int MaxScorePL { get; set; }
        public int AttemptsPL { get; set; }
        public bool PLPassed { get; set; }
        public bool IsAssessed { get; set; }
        public bool PLLocked { get; set; }
        public bool HasLearning { get; set; }
    }
}
