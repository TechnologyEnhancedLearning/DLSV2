namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public class ReportsViewModel
    {
        public ActivityTableViewModel ActivityTableViewModel { get; set; }
    }

    public class ActivityTableViewModel
    {
        public IEnumerable<ActivityTableRow> Rows { get; set; }
    }

    public class ActivityTableRow
    {
        private static string[] MonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Nov", "Dec"};

        public ActivityTableRow(MonthOfActivity monthOfActivity)
        {
            Month = MonthNames[monthOfActivity.Month - 1];
            Completions = monthOfActivity.Completions;
            Evaluations = monthOfActivity.Evaluations;
            Registrations = monthOfActivity.Registrations;
        }

        public string? Month { get; set; }
        public int Completions { get; set; }
        public int Evaluations { get; set; }
        public int Registrations { get; set; }
    }
}
