namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class ConfirmationVerifyEmailWarningViewModel
    {
        public ConfirmationVerifyEmailWarningViewModel(
            string? primaryEmailIfUnverified,
            string? centreEmailIfUnverified,
            string centreName
        )
        {
            PrimaryEmailIfUnverified = primaryEmailIfUnverified;
            CentreEmailIfUnverified = centreEmailIfUnverified;
            CentreName = centreName;
            NumberOfUnverifiedEmails =
                (PrimaryEmailIfUnverified == null ? 0 : 1) + (PrimaryEmailIfUnverified == null ? 0 : 1);
        }

        public string? PrimaryEmailIfUnverified { get; }
        public string? CentreEmailIfUnverified { get; }
        public string CentreName { get; }
        public int NumberOfUnverifiedEmails { get; }
    }
}
