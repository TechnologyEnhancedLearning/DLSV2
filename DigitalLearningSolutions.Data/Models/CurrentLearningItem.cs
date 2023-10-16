namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class CurrentLearningItem : StartedLearningItem
    {
        public DateTime? CompleteByDate { get; set; }
        public int EnrolmentMethodId { get; set; }
    }
}
