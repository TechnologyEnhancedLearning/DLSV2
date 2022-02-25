namespace DigitalLearningSolutions.Data.Models.DelegateGroups
{
    using System;

    public class GroupDelegate
    {
        public int GroupDelegateId { get; set; }

        public int GroupId { get; set; }

        public int DelegateId { get; set; }

        public string? FirstName { get; set; }

        public string LastName { get; set; }

        public string? EmailAddress { get; set; }

        public string CandidateNumber { get; set; }

        public DateTime AddedDate { get; set; }

        public string? ProfessionalRegistrationNumber { get; set; }

        public string Name => $"{FirstName} {LastName}";
    }
}
