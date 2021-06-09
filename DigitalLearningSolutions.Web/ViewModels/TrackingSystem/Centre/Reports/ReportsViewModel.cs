namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Collections.Generic;

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
        public string? Month { get; set; }
        public int Completions { get; set; }
        public int Evaluations { get; set; }
        public int Registrations { get; set; }
    }
}
