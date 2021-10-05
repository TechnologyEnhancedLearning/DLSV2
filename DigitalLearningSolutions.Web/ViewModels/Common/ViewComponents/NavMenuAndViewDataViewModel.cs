namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class NavMenuAndViewDataViewModel
    {
        public NavMenuAndViewDataViewModel(
            string selectedTab,
            ApplicationType application
        )
        {
            SelectedTab = selectedTab;
            Application = application;
        }

        public string SelectedTab { get; set; }
        public ApplicationType Application { get; set; }
    }
}
