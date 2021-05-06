namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem
{
    public class CentreConfigurationViewModel
    {
        public CentreConfigurationViewModel(string centreName, string regionName)
        {
            CentreName = centreName;
            RegionName = regionName;
        }

        public string CentreName { get; set; }
        public string RegionName { get; set; }
    }
}
