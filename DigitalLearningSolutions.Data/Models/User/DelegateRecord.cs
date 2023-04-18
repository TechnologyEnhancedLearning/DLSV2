namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Models.DelegateUpload;

    public class DelegateRecord
    {
        public DelegateRecord(DelegateTableRow row, int centreId, bool approved)
        {
            CentreId = centreId;
            CandidateNumber = row.CandidateNumber;
            FirstName = row.FirstName;
            LastName = row.LastName!;
            JobGroupId = row.JobGroupId!.Value;
            Active = row.Active!.Value;
            Answer1 = row.Answer1;
            Answer2 = row.Answer2;
            Answer3 = row.Answer3;
            Answer4 = row.Answer4;
            Answer5 = row.Answer5;
            Answer6 = row.Answer6;
            Approved = approved;
            Email = row.Email!;
        }

        public int CentreId { get; set; }
        public string? CandidateNumber { get; set; }
        public string? FirstName { get; set; }
        public string LastName { get; set; }
        public int JobGroupId { get; set; }
        public bool Active { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Answer6 { get; set; }
        public bool Approved { get; set; }
        public string Email { get; set; }
    }
}
