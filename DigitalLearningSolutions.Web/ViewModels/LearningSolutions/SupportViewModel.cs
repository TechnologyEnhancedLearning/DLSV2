namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportViewModel
    {
        public SupportViewModel(ApplicationType application, SupportPage currentPage)
        {
            CurrentPage = currentPage;

            Application = application;
        }

        public SupportPage CurrentPage { get; set; }
        public ApplicationType Application { get; set; }
    }
}
