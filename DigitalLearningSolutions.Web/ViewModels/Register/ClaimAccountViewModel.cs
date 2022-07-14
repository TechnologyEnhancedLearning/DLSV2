namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class ClaimAccountViewModel
    {
        public string CentreName { get; set; } = null!;
        public string CentreSpecificEmail { get; set; } = null!;
        public string RegistrationConfirmationHash { get; set; } = null!;
        public string DelegateId { get; set; } = null!;
        public string? SupportEmail { get; set; }
        public bool UserExists { get; set; }
        public bool UserActive { get; set; }
        public bool PasswordSet { get; set; }
    }
}
