namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class LogoViewComponent : ViewComponent
    {
        private readonly ILogoService logoService;

        public LogoViewComponent(ILogoService logoService)
        {
            this.logoService = logoService;
        }

        public IViewComponentResult Invoke(int? customisationId)
        {
            var centreId = UserClaimsPrincipal.GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);
            var customLogo = logoService.GetLogo(centreId, customisationId);

            var model = new LogoViewModel(customLogo);
            return View(model);
        }
    }
}
