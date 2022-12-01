namespace DigitalLearningSolutions.Web.Controllers.LearningSolutions
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.LearningSolutions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class LearningSolutionsController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly IConfigDataService configDataService;
        private readonly ILogger<LearningSolutionsController> logger;

        public LearningSolutionsController(
            IConfigDataService configDataService,
            ILogger<LearningSolutionsController> logger,
            ICentresDataService centresDataService
        )
        {
            this.configDataService = configDataService;
            this.logger = logger;
            this.centresDataService = centresDataService;
        }

        public IActionResult AccessibilityHelp()
        {
            var accessibilityText = configDataService.GetConfigValue(ConfigDataService.AccessibilityHelpText);
            if (accessibilityText == null)
            {
                logger.LogError("Accessibility text from Config table is null");
                return StatusCode(500);
            }

            var model = new AccessibilityHelpViewModel(accessibilityText);
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

            var model = new TermsViewModel(termsText);
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

            var model = new ContactViewModel(contactText);
            return View(model);
        }

        public IActionResult Error()
        {
            var model = GetErrorModel();
            Response.StatusCode = 500;
            return View("Error/UnknownError", model);
        }

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
            var centreId = User.GetCentreIdKnownNotNull();
            var bannerText = centresDataService.GetBannerText(centreId);
            return bannerText;
        }
    }
}
