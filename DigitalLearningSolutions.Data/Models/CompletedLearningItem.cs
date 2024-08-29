namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class CompletedLearningItem : StartedLearningItem
    {
        public DateTime Completed { get; set; }
        public DateTime? Evaluated { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public DateTime? RemovedDate { get; set; }
        public bool Published { get; set; }
    }
}
