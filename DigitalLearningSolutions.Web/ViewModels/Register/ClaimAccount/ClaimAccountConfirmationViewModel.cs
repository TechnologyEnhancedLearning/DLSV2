namespace DigitalLearningSolutions.Web.ViewModels.Register.ClaimAccount
{
    public class ClaimAccountConfirmationViewModel
    {
        public string CentreName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string CandidateNumber { get; set; } = null!;
        public bool WasPasswordSetByAdmin { get; set; }
    }
}
