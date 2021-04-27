namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class BackLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string aspController, string aspAction)
        {
            return View(new LinkViewModel(aspController, aspAction));
        }
    }
}
