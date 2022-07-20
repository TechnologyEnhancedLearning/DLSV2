namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class ClaimAccountViewModel
    {
        public int UserId { get; set; }
        public int CentreId { get; set; }
        public string CentreName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RegistrationConfirmationHash { get; set; } = null!;
        public string CandidateNumber { get; set; } = null!;
        public string? SupportEmail { get; set; }
        public bool EmailIsTaken { get; set; }
        public bool EmailIsTakenByActiveUser { get; set; }
        public bool PasswordSet { get; set; }
    }
}
