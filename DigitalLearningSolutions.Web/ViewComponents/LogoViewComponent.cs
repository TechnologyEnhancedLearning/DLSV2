namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public class LogoViewComponent : ViewComponent
    {
        private readonly ILogoService logoService;

        public LogoViewComponent(ILogoService logoService)
        {
            this.logoService = logoService;
        }

        public IViewComponentResult Invoke(int? customisationId)
        {
            var centreId = ((ClaimsPrincipal) User).GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);
            if (centreId == null)
            {
                return View(new LogoViewModel(null));
            }

            var customLogo = logoService.GetLogo(centreId, customisationId);
            var model = new LogoViewModel(customLogo);
            return View(model);
        }
    }
}
