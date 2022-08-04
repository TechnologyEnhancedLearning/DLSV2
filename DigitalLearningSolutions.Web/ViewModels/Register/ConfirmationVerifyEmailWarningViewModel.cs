namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class ConfirmationVerifyEmailWarningViewModel
    {
        public ConfirmationVerifyEmailWarningViewModel(
            string? unverifiedPrimaryEmail,
            string? unverifiedCentreEmail,
            string centreName
        )
        {
            UnverifiedPrimaryEmail = unverifiedPrimaryEmail;
            UnverifiedCentreEmail = unverifiedCentreEmail;
            CentreName = centreName;
            NumberOfUnverifiedEmails =
                (UnverifiedPrimaryEmail == null ? 0 : 1) + (UnverifiedPrimaryEmail == null ? 0 : 1);
        }

        public string? UnverifiedPrimaryEmail { get; }
        public string? UnverifiedCentreEmail { get; }
        public string CentreName { get; }
        public int NumberOfUnverifiedEmails { get; }
    }
}
