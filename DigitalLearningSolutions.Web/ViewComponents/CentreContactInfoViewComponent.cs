namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class CentreContactInfoViewComponent : ViewComponent
    {
        private readonly ICentresService centresService;

        public CentreContactInfoViewComponent(ICentresService centresService)
        {
            this.centresService = centresService;
        }

        public IViewComponentResult Invoke(int centreId)
        {
            var bannerText = centresService.GetBannerText(centreId);
            var model = new CentreContactInfoViewModel(centreId, bannerText);
            return View(model);
        }
    }
}
