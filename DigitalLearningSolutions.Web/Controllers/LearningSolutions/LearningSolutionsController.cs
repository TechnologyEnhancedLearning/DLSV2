namespace DigitalLearningSolutions.Web.Controllers.LearningSolutions
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.LearningSolutions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using System;
    using System.Linq;
    using System.Reflection.PortableExecutable;

    public class LearningSolutionsController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ICentresService centresService;
        private readonly IConfigDataService configDataService;
        private readonly ILogger<LearningSolutionsController> logger;

        public LearningSolutionsController(
            IConfigDataService configDataService,
            ILogger<LearningSolutionsController> logger,
            ICentresDataService centresDataService,
            ICentresService centresService
        )
        {
            this.configDataService = configDataService;
            this.logger = logger;
            this.centresDataService = centresDataService;
            this.centresService = centresService;
        }

        public IActionResult AccessibilityHelp()
        {
            var accessibilityText = configDataService.GetConfigValue(ConfigDataService.AccessibilityHelpText);
            if (accessibilityText == null)
            {
                logger.LogError("Accessibility text from Config table is null");
                return StatusCode(500);
            }

            DateTime lastUpdatedDate = DateTime.Now;
            DateTime nextReviewDate = DateTime.Now;

            lastUpdatedDate = configDataService.GetConfigLastUpdated(ConfigDataService.AccessibilityHelpText);
            nextReviewDate = lastUpdatedDate.AddYears(3);

            var model = new AccessibilityHelpViewModel(accessibilityText, lastUpdatedDate, nextReviewDate);
            return View(model);
        }

        public IActionResult Terms()
        {
            var termsText = configDataService.GetConfigValue(ConfigDataService.TermsText);
            if (termsText == null)
            {
                logger.LogError("Terms text from Config table is null");
                return StatusCode(500);
            }
            DateTime lastUpdatedDate = DateTime.Now;
            DateTime nextReviewDate = DateTime.Now;

            lastUpdatedDate = configDataService.GetConfigLastUpdated(ConfigDataService.TermsText);
            nextReviewDate = lastUpdatedDate.AddYears(3);
            var model = new TermsViewModel(termsText, lastUpdatedDate, nextReviewDate);
            return View(model);
        }

        public IActionResult Contact()
        {
            var contactText = configDataService.GetConfigValue(ConfigDataService.ContactText);
            if (contactText == null)
            {
                logger.LogError("Contact text from Config table is null");
                return StatusCode(500);
            }
            var centreId = User.GetCentreId();
            if (centreId.GetValueOrDefault() > 0)
            {
                var centreSummary = centresService.GetCentreSummaryForContactDisplay(centreId.Value);
                return View(new ContactViewModel(contactText, centreSummary));
            }

            var model = new ContactViewModel(contactText);
            return View(model);
        }

        public IActionResult Error()
        {
            var model = GetErrorModel();
            Response.StatusCode = 500;
            return View("Error/UnknownError", model);
        }

        [NoCaching]
        [Route("/LearningSolutions/StatusCode/{code:int}")]
        [IgnoreAntiforgeryToken]
        public new IActionResult StatusCode(int code)
        {
            var model = GetErrorModel();
            Response.StatusCode = code;

            return code switch
            {
                404 => View("Error/PageNotFound", model),
                403 => View("Error/Forbidden", model),
                410 => View("Error/Gone", model),
                _ => View("Error/UnknownError", model),
            };
        }

        [Route("/AccessDenied")]
        [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
        public IActionResult AccessDenied()
        {
            if (User.IsDelegateOnlyAccount())
            {
                return RedirectToAction("AccessDenied", "LearningPortal");
            }

            return View("Error/AccessDenied");
        }

        [Route("/PleaseLogout")]
        [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
        public IActionResult PleaseLogout()
        {
            return View();
        }

        private ErrorViewModel GetErrorModel()
        {
            try
            {
                var bannerText = GetBannerText();
                return new ErrorViewModel(bannerText);
            }
            catch
            {
                return new ErrorViewModel(null);
            }
        }

        private string? GetBannerText()
        {
            string? bannerText = null;
            var centreId = User.GetCentreId();
            if (centreId != null)
            {
                bannerText = centresDataService.GetBannerText((int)centreId);
            }
            return bannerText;
        }

        public IActionResult AcceptableUsePolicy()
        {
            var acceptableUsePolicyText = configDataService.GetConfigValue(ConfigDataService.AcceptableUsePolicyText);

            if (acceptableUsePolicyText == null)
            {
                logger.LogError("‘Acceptable Use Policy text from Config table is null");
                return StatusCode(500);
            }
            DateTime lastUpdatedDate = DateTime.Now;
            DateTime nextReviewDate = DateTime.Now;

            lastUpdatedDate = configDataService.GetConfigLastUpdated(ConfigDataService.AcceptableUsePolicyText);
            nextReviewDate = lastUpdatedDate.AddYears(3);
            var model = new AcceptableUsePolicyViewModel(acceptableUsePolicyText, lastUpdatedDate, nextReviewDate);
            return View(model);
        }
        public IActionResult PrivacyNotice()
        {
            var PrivacyPolicyText = configDataService.GetConfigValue(ConfigDataService.PrivacyPolicyText);
            if (PrivacyPolicyText == null)
            {
                logger.LogError("PrivacyPolicy text from Config table is null");
                return StatusCode(500);
            }

            DateTime lastUpdatedDate = DateTime.Now;
            DateTime nextReviewDate = DateTime.Now;

            lastUpdatedDate = configDataService.GetConfigLastUpdated(ConfigDataService.PrivacyPolicyText);
            nextReviewDate = lastUpdatedDate.AddYears(3);

            var model = new PrivacyNoticeViewModel(PrivacyPolicyText, lastUpdatedDate, nextReviewDate);
            return View(model);
        }

        [Route("/TooManyRequests")]
        [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
        public IActionResult TooManyRequests()
        {
            return View("Error/TooManyRequests");
        }
    }
}
