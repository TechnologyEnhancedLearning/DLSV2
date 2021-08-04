namespace DigitalLearningSolutions.Data.Models.User
{
    public class DelegateRecord
    {
        public DelegateRecord(
            int centreId,
            string delegateId,
            string? firstName,
            string lastName,
            int jobGroupId,
            bool active,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6,
            string? aliasId,
            bool approved,
            string? email
        )
        {
            CentreId = centreId;
            DelegateId = delegateId;
            FirstName = firstName;
            LastName = lastName;
            JobGroupId = jobGroupId;
            Active = active;
            Answer1 = answer1;
            Answer2 = answer2;
            Answer3 = answer3;
            Answer4 = answer4;
            Answer5 = answer5;
            Answer6 = answer6;
            AliasId = aliasId;
            Approved = approved;
            Email = email;
        }

        public int CentreId { get; set; }
        public string DelegateId { get; set; }
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
        public string? AliasId { get; set; }
        public bool Approved { get; set; }
        public string? Email { get; set; }
    }
}
