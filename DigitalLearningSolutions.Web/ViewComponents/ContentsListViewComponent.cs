namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.MiniHub;
    using Microsoft.AspNetCore.Mvc;

    public class ContentsListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(MiniHubNavigationModel miniHubNavigationModel)
        {
            return View(miniHubNavigationModel);
        }
    }
}
