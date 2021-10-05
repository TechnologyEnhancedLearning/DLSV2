namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class NavMenuAndViewDataViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string selectedTab,
            ApplicationType application
        )
        {
            var navMenuViewModel = new NavMenuAndViewDataViewModel(selectedTab, application);
            return View(navMenuViewModel);
        }
    }
}
