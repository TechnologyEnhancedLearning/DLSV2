namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
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
        private static string[] MonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};

        public ActivityTableRow(MonthOfActivity monthOfActivity)
        {
            Period = DateTime.Parse($"{monthOfActivity.Year}-{monthOfActivity.Month}-01").ToString("MMMM, yyyy");
            Completions = monthOfActivity.Completions;
            Evaluations = monthOfActivity.Evaluations;
            Registrations = monthOfActivity.Registrations;
        }

        public string? Period { get; set; }
        public int Completions { get; set; }
        public int Evaluations { get; set; }
        public int Registrations { get; set; }
    }
}
