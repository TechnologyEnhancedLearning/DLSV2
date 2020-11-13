namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;

    public class HeaderViewComponent : ViewComponent
    {
        private readonly ICentresService centresService;

        public HeaderViewComponent(ICentresService centresService)
        {
            this.centresService = centresService;
        }

        public IViewComponentResult Invoke()
        {
            var centreId = UserClaimsPrincipal.GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);
            if (centreId == null)
            {
                return View(new HeaderViewModel(null));
            }

            var customLogo = centresService.GetCentreLogo(centreId.Value);
            if (customLogo.LogoUrl == null)
            {
                return View(new HeaderViewModel(null));
            }

            var model = new HeaderViewModel(customLogo);
            return View(model);
        }
    }
}
