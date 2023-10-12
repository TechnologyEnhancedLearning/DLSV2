namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System;
    public class DashboardToDoItem
    {
        public int? FrameworkID { get; set; }
        public int? RoleProfileID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string RequestorName { get; set; } = string.Empty;
        public bool SignOffRequired { get; set; }
        public DateTime Requested { get; set; }
    }
}
