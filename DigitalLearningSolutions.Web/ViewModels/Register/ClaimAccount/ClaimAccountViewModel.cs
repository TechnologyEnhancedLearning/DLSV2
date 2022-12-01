namespace DigitalLearningSolutions.Web.ViewModels.Register.ClaimAccount
{
    public class ClaimAccountViewModel : IHasDataForDelegateRecordSummary
    {
        public int UserId { get; set; }
        public int CentreId { get; set; }
        public string CentreName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RegistrationConfirmationHash { get; set; } = null!;
        public string CandidateNumber { get; set; } = null!;
        public string? SupportEmail { get; set; }
        public int? IdOfUserMatchingEmailIfAny { get; set; }
        public bool UserMatchingEmailIsActive { get; set; }
        public bool WasPasswordSetByAdmin { get; set; }
    }
}
