namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class CompletedLearningItem : StartedLearningItem
    {
        public DateTime Completed { get; set; }
        public DateTime? Evaluated { get; set; }
        public DateTime? ArchivedDate { get; set; }
    }
}
