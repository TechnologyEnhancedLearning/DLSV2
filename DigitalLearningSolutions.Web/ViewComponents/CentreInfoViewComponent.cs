namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class CentreInfoViewComponent: ViewComponent
    {
        private readonly ICentresDataService centresDataService;

        public CentreInfoViewComponent(ICentresDataService centresDataService)
        {
            this.centresDataService = centresDataService;
        }

        public IViewComponentResult Invoke(int centreId)
        {
            var bannerText = centresDataService.GetBannerText(centreId);
            var model = new CentreInfoViewModel(centreId, bannerText);
            return View(model);
        }
    }
}
