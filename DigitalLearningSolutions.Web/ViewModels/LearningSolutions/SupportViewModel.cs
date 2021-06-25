namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportViewModel
    {
        public SupportViewModel(ApplicationType application, SupportPage currentPage)
        {
            CurrentPage = currentPage;

            Application = application;

            HeaderPath = Application.Equals(ApplicationType.TrackingSystem)
                ? $"{ConfigHelper.GetAppConfig()["AppRootPath"]}/TrackingSystem/Centre/Dashboard"
                : null;
        }

        public string? HeaderPath { get; set; }
        public SupportPage CurrentPage { get; set; }
        public ApplicationType Application { get; set; }
    }
}
