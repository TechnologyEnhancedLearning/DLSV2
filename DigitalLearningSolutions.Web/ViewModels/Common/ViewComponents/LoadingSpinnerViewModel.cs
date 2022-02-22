namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class LoadingSpinnerViewModel
    {
        public LoadingSpinnerViewModel(bool pageHasSideNavMenu)
        {
            PageHasSideNavMenu = pageHasSideNavMenu;
        }

        public bool PageHasSideNavMenu { get; set; }
    }
}
