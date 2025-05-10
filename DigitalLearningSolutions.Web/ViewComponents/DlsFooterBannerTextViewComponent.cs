using System.Security.Claims;
using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
using DigitalLearningSolutions.Web.Services;
using System;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewComponents
{
    public class DlsFooterBannerTextViewComponent : ViewComponent
    {
        private readonly ICentresService centresService;

        public DlsFooterBannerTextViewComponent(ICentresService centresService)
        {
            this.centresService = centresService;
        }

        public  IViewComponentResult Invoke()
        {
            var centreId = ((ClaimsPrincipal)User).GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);
            if (centreId == null)
            {
                return View(new DlsFooterBannerTextViewModel(null));
            }

            var bannerText = centresService.GetBannerText(Convert.ToInt32(centreId));
            var model = new DlsFooterBannerTextViewModel(bannerText);
            return View(model);
        }
    }
}
