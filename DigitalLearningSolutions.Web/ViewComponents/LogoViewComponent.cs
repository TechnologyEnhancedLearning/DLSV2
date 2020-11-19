namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class LogoViewComponent : ViewComponent
    {
        private readonly ICentresService centresService;

        public LogoViewComponent(ICentresService centresService)
        {
            this.centresService = centresService;
        }

        public IViewComponentResult Invoke()
        {
            var centreId = UserClaimsPrincipal.GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);
            if (centreId == null)
            {
                return View(new LogoViewModel(null));
            }

            var customLogo = centresService.GetCentreLogo(centreId.Value);
            if (customLogo.LogoUrl == null)
            {
                return View(new LogoViewModel(null));
            }

            var model = new LogoViewModel(customLogo);
            return View(model);
        }
    }
}
