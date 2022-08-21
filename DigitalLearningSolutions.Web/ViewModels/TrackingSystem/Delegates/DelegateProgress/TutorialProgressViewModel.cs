namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Progress;

    public class TutorialProgressViewModel
    {
        public TutorialProgressViewModel(DetailedTutorialProgress tutorial)
        {
            TutorialName = tutorial.TutorialName;
            TutorialStatus = tutorial.TutorialStatus;
            TimeTaken = tutorial.TimeTaken;
            AvgTime = tutorial.AvgTime;
            DiagnosticScore = tutorial.DiagnosticScore;
            PossibleScore = tutorial.PossibleScore;
        }

        public string TutorialName { get; set; }
        public string TutorialStatus { get; set; }
        public int TimeTaken { get; set; }
        public int AvgTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public int PossibleScore { get; set; }
    }
}
