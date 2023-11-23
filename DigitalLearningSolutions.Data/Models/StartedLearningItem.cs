namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public abstract class StartedLearningItem : BaseLearningItem
    {
        public DateTime StartedDate { get; set; }
        public DateTime? LastAccessed { get; set; }
        public int? DiagnosticScore { get; set; }
        public int Passes { get; set; }
        public int Sections { get; set; }
        public int ProgressID { get; set; }
        public string CentreName { get; set; }
    }
}
