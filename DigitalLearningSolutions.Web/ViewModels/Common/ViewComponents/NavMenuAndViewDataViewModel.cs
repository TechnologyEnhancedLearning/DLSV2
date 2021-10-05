namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class NavMenuAndViewDataViewModel
    {
        public NavMenuAndViewDataViewModel(
            Tab selectedTab,
            ApplicationType application
        )
        {
            SelectedTab = selectedTab;
            Application = application;
        }

        public Tab SelectedTab { get; set; }
        public ApplicationType Application { get; set; }
    }
}
