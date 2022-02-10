namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class TopLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string linkText,
            string topElementId
        )
        {
            return View(new TopLinkViewModel(linkText, topElementId));
        }
    }
}
