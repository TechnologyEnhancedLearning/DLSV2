namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class CentreContactInfoViewComponent : ViewComponent
    {
        private readonly ICentresDataService centresDataService;

        public CentreContactInfoViewComponent(ICentresDataService centresDataService)
        {
            this.centresDataService = centresDataService;
        }

        public IViewComponentResult Invoke(int centreId)
        {
            var bannerText = centresDataService.GetBannerText(centreId);
            var model = new CentreContactInfoViewModel(centreId, bannerText);
            return View(model);
        }
    }
}
