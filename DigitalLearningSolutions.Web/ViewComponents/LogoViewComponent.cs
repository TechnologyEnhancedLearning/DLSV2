namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Security.Claims;
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
            // If the user is not logged in, render nothing for the logo
            if (User.Identity is null || !User.Identity.IsAuthenticated)
            {
                return View(new LogoViewModel(null));
            }
            var centreId = ((ClaimsPrincipal) User).GetCentreId();
            var customLogo = logoService.GetLogo(centreId, customisationId);

            var model = new LogoViewModel(customLogo);
            return View(model);
        }
    }
}
