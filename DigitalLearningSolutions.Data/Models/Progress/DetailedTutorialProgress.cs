namespace DigitalLearningSolutions.Data.Models.Progress
{
    public class DetailedTutorialProgress
    {
        public string TutorialName { get; set; } = string.Empty;
        public string TutorialStatus { get; set; } = string.Empty;
        public int TimeTaken { get; set; }
        public int AvgTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public int PossibleScore { get; set; }
    }
}
