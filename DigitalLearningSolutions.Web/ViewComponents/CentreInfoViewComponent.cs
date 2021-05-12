namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class CentreInfoViewComponent: ViewComponent
    {
        public IViewComponentResult Invoke(int centreId, string bannerText)
        {
            var model = new CentreInfoViewModel(centreId, bannerText);
            return View(model);
        }
    }
}
