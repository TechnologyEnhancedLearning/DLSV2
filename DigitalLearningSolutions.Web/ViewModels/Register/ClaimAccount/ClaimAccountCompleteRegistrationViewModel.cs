namespace DigitalLearningSolutions.Web.ViewModels.Register.ClaimAccount
{
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class ClaimAccountCompleteRegistrationViewModel : ConfirmPasswordViewModel, IHasDataForDelegateRecordSummary
    {
        public string CentreName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public bool WasPasswordSetByAdmin { get; set; }
    }
}
