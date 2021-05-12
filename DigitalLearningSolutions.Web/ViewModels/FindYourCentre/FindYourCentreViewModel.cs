namespace DigitalLearningSolutions.Web.ViewModels.FindYourCentre
{
    public class FindYourCentreViewModel
    {
        public string CentreId { get; set; }

        public FindYourCentreViewModel() { }

        public FindYourCentreViewModel(string centreId)
        {
            CentreId = centreId;
        }
    }
}
