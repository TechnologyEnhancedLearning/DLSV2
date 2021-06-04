namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    public class CentreAdministratorViewModel
    {
        public CentreAdministratorViewModel(int centreId)
        {
            CentreId = centreId;
        }

        public int CentreId { get; set; }
    }
}
