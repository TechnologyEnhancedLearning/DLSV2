namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class CurrentLearningItem : StartedLearningItem
    {
        public DateTime? CompleteByDate { get; set; }
    }
}
