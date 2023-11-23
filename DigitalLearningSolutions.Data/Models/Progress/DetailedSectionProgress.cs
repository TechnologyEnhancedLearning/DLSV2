namespace DigitalLearningSolutions.Data.Models.Progress
{
    using System.Collections.Generic;

    public class DetailedSectionProgress
    {
        public string SectionName { get; set; }
        public int SectionId { get; set; }

        public int Completion { get; set; }
        public int TotalTime { get; set; }
        public int AverageTime { get; set; }
        public string? PostLearningAssessPath { get; set; }
        public bool IsAssessed { get; set; }
        public int Outcome { get; set; }
        public int Attempts { get; set; }
        public bool Passed { get; set; }

        public IEnumerable<DetailedTutorialProgress>? Tutorials { get; set; }
    }
}