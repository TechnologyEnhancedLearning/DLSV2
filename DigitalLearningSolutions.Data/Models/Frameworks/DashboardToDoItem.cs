namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System;
    public class DashboardToDoItem
    {
        public int? FrameworkID { get; set; }
        public int? RoleProfileID { get; set; }
        public string ItemName { get; set; }
        public string RequestorName { get; set; }
        public bool SignOffRequired { get; set; }
        public DateTime Requested { get; set; }
    }
}
