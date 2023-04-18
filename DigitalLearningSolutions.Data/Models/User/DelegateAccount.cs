namespace DigitalLearningSolutions.Data.Models.User
{
    using System;

    public class DelegateAccount
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool Active { get; set; }
        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public bool CentreActive { get; set; }
        public string CandidateNumber { get; set; }
        public DateTime DateRegistered { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Answer6 { get; set; }
        public bool Approved { get; set; }
        public bool ExternalReg { get; set; }
        public bool SelfReg { get; set; }
        public string? OldPassword { get; set; }
        public DateTime? CentreSpecificDetailsLastChecked { get; set; }
        public string? RegistrationConfirmationHash { get; set; }
        public DateTime? RegistrationConfirmationHashCreationDateTime { get; set; }

        public bool IsYetToBeClaimed => RegistrationConfirmationHash != null;
    }
}
