namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Progress;

    public class SectionProgressViewModel
    {
        public SectionProgressViewModel(DetailedSectionProgress section)
        {
            SectionName = section.SectionName;
            Completion = section.Completion;
            TotalTime = section.TotalTime;
            AverageTime = section.AverageTime;
            PostLearningAssessment = !string.IsNullOrWhiteSpace(section.PostLearningAssessPath) && section.IsAssessed;
            Outcome = section.Outcome;
            Attempts = section.Attempts;
            Passed = section.Passed;

            Tutorials = section.Tutorials?.Select(t => new TutorialProgressViewModel(t)) ??
                        new List<TutorialProgressViewModel>();
        }

        public string SectionName { get; set; }
        public int Completion { get; set; }
        public int TotalTime { get; set; }
        public int AverageTime { get; set; }
        public bool PostLearningAssessment { get; set; }
        public int Outcome { get; set; }
        public int Attempts { get; set; }
        public bool Passed { get; set; }

        public IEnumerable<TutorialProgressViewModel> Tutorials { get; set; }
    }
}
