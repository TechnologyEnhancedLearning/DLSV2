namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.MiniHub;
    using Microsoft.AspNetCore.Mvc;

    public class PaginationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(MiniHubNavigationModel miniHubNavigationModel)
        {
            return View(miniHubNavigationModel);
        }
    }
}
