namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System;

    public class ActivityLogDetail
    {
        public DateTime LogDate { get; set; }
        public string? CourseName { get; set; }
        public string? CustomisationName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PrimaryEmail { get; set; }
        public string? DelegateId { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Answer6 { get; set; }
        public bool Enrolled { get; set; }
        public bool Completed { get; set; }
        public bool Evaluated { get; set; }
    }
}
