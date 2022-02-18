namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class LoadingSpinnerViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(bool pageHasSideNavMenu)
        {
            var model = new LoadingSpinnerViewModel(pageHasSideNavMenu);
            return View(model);
        }
    }
}
