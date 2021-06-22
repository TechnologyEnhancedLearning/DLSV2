namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportViewModel
    {
        public SupportViewModel(string applicationName, SupportPage currentPage)
        {
            CurrentPage = currentPage;

            Application = Enumeration.FromName<ApplicationType>(applicationName);

            HeaderPath = Application.Equals(ApplicationType.TrackingSystem)
                ? $"{ConfigHelper.GetAppConfig()["AppRootPath"]}/TrackingSystem/Centre/Dashboard"
                : null;
        }

        public string? HeaderPath { get; set; }
        public SupportPage CurrentPage { get; set; }
        public ApplicationType Application { get; set; }
    }
}
