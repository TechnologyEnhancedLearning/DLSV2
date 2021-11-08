namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System;

    public class DelegateProgressEditCompletionDateViewModel
    {
        public DelegateProgressEditCompletionDateViewModel() { }

        public DelegateProgressEditCompletionDateViewModel(int progressId, string customisationName, int? day, int? month, int? year)
        {
            ProgressId = progressId;
            CustomisationName = customisationName;
            Day = day;
            Month = month;
            Year = year;
        }
        public int ProgressId { get; set; }
        public string CustomisationName { get; set; }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

    }
}
