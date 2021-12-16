namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using System;
    using DigitalLearningSolutions.Data.Models;

    public class CompletedLearningItemViewModel : StartedLearningItemViewModel
    {
        public CompletedLearningItemViewModel(CompletedLearningItem item) : base(item)
        {
            CompletedDate = item.Completed;
            EvaluatedDate = item.Evaluated;
            ArchivedDate = item.ArchivedDate;
        }

        public DateTime CompletedDate { get; }
        public DateTime? EvaluatedDate { get; }
        public DateTime? ArchivedDate { get; }
    }
}
