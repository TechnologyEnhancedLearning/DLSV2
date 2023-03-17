using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Extensions;
using DigitalLearningSolutions.Data.Utilities;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DigitalLearningSolutions.Web.Controllers.LearningSolutions
{
    public class CookieConsentController : Controller
    {
        private readonly IConfigDataService configDataService;
        private readonly IConfiguration configuration;
        private readonly IClockUtility clockUtility;
        private readonly ILogger<CookieConsentController> logger;
        private string CookieBannerConsentCookieName = "";
        private int CookieBannerConsentCookieExpiryDays=0;

        public CookieConsentController(
            IConfigDataService configDataService,
            IConfiguration configuration,
            IClockUtility clockUtility,
            ILogger<CookieConsentController> logger
       )
        {
            this.configDataService = configDataService;
            this.configuration = configuration;
            this.clockUtility = clockUtility;
            this.logger = logger;
            CookieBannerConsentCookieName = configuration.GetCookieBannerConsentCookieName();
            CookieBannerConsentCookieExpiryDays = configuration.GetCookieBannerConsentExpiryDays();
        }
        public IActionResult CookiePolicy()
        {
            var cookiePolicyContent = configDataService.GetConfigValue(ConfigDataService.CookiePolicyContent);
            var policyLastUpdatedDate = configDataService.GetConfigValue(ConfigDataService.CookiePolicyUpdatedDate);
            if (cookiePolicyContent == null)
            {
                logger.LogError("Cookie policy content from Config table is null");
                return StatusCode(500);
            }

            var model = new CookieConsentViewModel(cookiePolicyContent);
            model.PolicyUpdatedDate = policyLastUpdatedDate;
            if (Request != null)
            {
                if (Request.Cookies.HasDLSBannerCookie(CookieBannerConsentCookieName,"true"))
                    model.UserConsent = "true";
                else if (Request.Cookies.HasDLSBannerCookie(CookieBannerConsentCookieName, "false"))
                    model.UserConsent = "false";
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult CookiePolicy(CookieConsentViewModel model)
        {
            string consent = model.UserConsent?.ToString();
            if (!string.IsNullOrEmpty(consent))
                ConfirmCookieConsent(consent);

            return View("CookieConfirmation");
        }

        // [HttpPost]
        public IActionResult CookieConsentConfirmation(string consent, string path)
        {
            if (!string.IsNullOrEmpty(consent))
                ConfirmCookieConsent(consent, true);

            string controllerName = string.Empty;
            string actionName = string.Empty;
            string routeValue = string.Empty;

            string[] strTemp = path.Split('/');

            for (int i = 0; i < strTemp.Length; i++)
            {
                if (i == 1) controllerName = strTemp[i] ?? "Home";
                if (i == 2) actionName = strTemp[i] ?? "Index";
                if (i == 3) routeValue = strTemp[i];
            }

            return RedirectToAction(actionName, controllerName);
        }

        public void ConfirmCookieConsent(string consent, bool setTempDataConsentViaBannerPost = false)
        {
            if (Response != null)
            {
                if (consent == "true")
                    Response.Cookies?.SetDLSBannerCookie(CookieBannerConsentCookieName, consent,
                        clockUtility.UtcNow.AddDays(CookieBannerConsentCookieExpiryDays));

                else if (consent == "false")
                    Response.Cookies?.SetDLSBannerCookie(CookieBannerConsentCookieName, consent,
                        clockUtility.UtcNow.AddDays(CookieBannerConsentCookieExpiryDays));

                TempData["userConsentCookieOption"] = consent;

                if (setTempDataConsentViaBannerPost) TempData["consentViaBannerPost"] = consent; // Need this tempdata to display the confirmation banner
            }
        }
    }
}
