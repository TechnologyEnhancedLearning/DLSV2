namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    public class CentreSpecificEmailDetailsViewModel
    {
        public CentreSpecificEmailDetailsViewModel(
            string primaryEmail,
            string? centreSpecificEmail,
            string? centreName
        )
        {
            PrimaryEmail = primaryEmail;
            CentreSpecificEmail = centreSpecificEmail;
            CentreName = centreName;
        }

        public string PrimaryEmail { get; set; }
        public string? CentreSpecificEmail { get; set; }
        public string? CentreName { get; set; }
    }
}
