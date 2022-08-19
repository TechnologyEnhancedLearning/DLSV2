namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class AdminConfirmationViewModel
    {
        public AdminConfirmationViewModel(
            string? primaryEmailIfUnverified,
            string? centreEmailIfUnverified,
            string centreName
        )
        {
            PrimaryEmailIfUnverified = primaryEmailIfUnverified;
            CentreEmailIfUnverified = centreEmailIfUnverified;
            CentreName = centreName;
        }

        public string? PrimaryEmailIfUnverified { get; }
        public string? CentreEmailIfUnverified { get; }
        public string CentreName { get; }
    }
}
