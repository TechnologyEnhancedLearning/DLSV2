namespace DigitalLearningSolutions.Data.Models.User
{
    using System;

    public class DelegateUser : User
    {
        public bool Approved { get; set; }
        public string CandidateNumber { get; set; }
        public DateTime? DateRegistered { get; set; }
        public string? JobGroupName { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Answer6 { get; set; }
    }
}
