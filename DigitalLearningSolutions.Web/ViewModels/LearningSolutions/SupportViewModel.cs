namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportViewModel
    {
        public SupportViewModel(string applicationName, SupportPage currentPage)
        {
            CurrentPage = currentPage;

            if (applicationName == "TrackingSystem")
            {
                ApplicationName = "Tracking System";
                HeaderPath = $"{ConfigHelper.GetAppConfig()["AppRootPath"]}/TrackingSystem/Centre/Dashboard";
            }
            else
            {
                ApplicationName = "Frameworks";
                HeaderPath = null;
            }
        }

        public string ApplicationName { get; set; }
        public string? HeaderPath { get; set; }
        public SupportPage CurrentPage { get; set; }
    }
}
