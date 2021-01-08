namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.Helpers;

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
            TimeSpentSummary = DurationFormattingHelper.FormatDuration(timeSpent);
            AverageTimeSummary = DurationFormattingHelper.FormatDuration(averageTimeSpent);
            ShowTime = showTimeSetting && showLearnStatusSetting;
        }
    }
}
