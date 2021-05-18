namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class BackLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string aspController, string aspAction, string linkText)
        {
            if (string.IsNullOrWhiteSpace(linkText))
            {
                linkText = "Go back";
            }
            return View(new LinkViewModel(aspController, aspAction, linkText));
        }
    }
}
