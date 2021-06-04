namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    public class CentreAdministratorsViewModel
    {
        public CentreAdministratorsViewModel(int centreId)
        {
            CentreId = centreId;
        }

        public int CentreId { get; set; }
    }
}
