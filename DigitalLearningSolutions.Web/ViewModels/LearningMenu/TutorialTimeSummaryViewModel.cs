namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    public class TutorialTimeSummaryViewModel
    {
        public string TimeSpentSummary { get; }
        public string AverageTimeSummary { get; }
        public bool ShowTime { get; }

        public TutorialTimeSummaryViewModel(
            int timeSpent,
            int averageTimeSpent,
            bool showTimeSetting,
            bool showLearnStatusSetting
        )
        {
            TimeSpentSummary = timeSpent == 1
                ? $"{timeSpent} minute spent"
                : $"{timeSpent} minutes spent";
            AverageTimeSummary = averageTimeSpent == 1
                ? $"(average tutorial time {averageTimeSpent} minute)"
                : $"(average tutorial time {averageTimeSpent} minutes)";
            ShowTime = showTimeSetting && showLearnStatusSetting;
        }
    }
}
