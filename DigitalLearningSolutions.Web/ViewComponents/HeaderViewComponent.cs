namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
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
                return View();
            }

            var customLogo = centresService.GetCentreLogo(centreId.Value);
            if (customLogo.LogoUrl == null)
            {
                return View();
            }

            var model = new HeaderViewModel(customLogo);
            return View(model);
        }
    }
}
