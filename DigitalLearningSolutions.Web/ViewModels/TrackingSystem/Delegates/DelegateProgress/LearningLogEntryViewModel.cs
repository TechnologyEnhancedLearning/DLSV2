namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System;
    using DigitalLearningSolutions.Data.Models;

    public class LearningLogEntryViewModel
    {
        public LearningLogEntryViewModel(LearningLogEntry entry)
        {
            When = entry.When;
            LearningTime = entry.LearningTime?.ToString() ?? "N/A";
            AssessmentTaken = entry.AssessmentTaken ?? "N/A";
            AssessmentScore = entry.AssessmentScore?.ToString() ?? "N/A";
            if (entry.AssessmentStatus == null)
            {
                AssessmentStatus = "N/A";
            }
            else
            {
                AssessmentStatus = entry.AssessmentStatus.Value ? "Pass" : "Fail";
            }
        }

        public DateTime When { get; set; }
        public string LearningTime { get; set; }
        public string AssessmentTaken { get; set; }
        public string AssessmentScore { get; set; }
        public string AssessmentStatus { get; set; }
    }
}
