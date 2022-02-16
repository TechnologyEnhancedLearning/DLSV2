namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class AllLearningLogEntriesViewModel
    {
        public AllLearningLogEntriesViewModel(IEnumerable<LearningLogEntry> entries)
        {
            LearningLogEntries = entries.Select(entry => new LearningLogEntryViewModel(entry));
        }

        public IEnumerable<LearningLogEntryViewModel> LearningLogEntries { get; set; }
    }
}
