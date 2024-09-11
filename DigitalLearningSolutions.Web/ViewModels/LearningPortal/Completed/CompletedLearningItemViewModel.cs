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
            RemovedDate = item.RemovedDate;
            CheckUnpublishedCourse = item.CheckUnpublishedCourse;
        }

        public DateTime CompletedDate { get; }
        public DateTime? EvaluatedDate { get; }
        public DateTime? ArchivedDate { get; }
        public DateTime? RemovedDate { get; }
        public int CheckUnpublishedCourse { get; }
    }
}
