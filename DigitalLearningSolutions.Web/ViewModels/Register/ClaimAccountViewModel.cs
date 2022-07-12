namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class ClaimAccountViewModel
    {
        public string CentreName { get; set; }
        public string CentreSpecificEmail { get; set; }
        public bool UserExists { get; set; }
        public bool UserActive { get; set; }
    }
}
