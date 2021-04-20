namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.MiniHub;
    using Microsoft.AspNetCore.Mvc;

    public class MiniHubHeadingViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(MiniHubNavigationModel miniHubNavigationModel)
        {
            return View(miniHubNavigationModel);
        }
    }
}
