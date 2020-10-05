namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserOnly)]
    public partial class LearningPortalController : Controller
    {
        private readonly ICentresService centresService;
        private readonly IConfigService configService;
        private readonly ICourseService courseService;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly IUnlockService unlockService;
        private readonly ILogger<LearningPortalController> logger;
        private readonly IConfiguration config;
        private readonly IFilteredApiHelperService filteredApiHelperService;

        public LearningPortalController(
            ICentresService centresService,
            IConfigService configService,
            ICourseService courseService,
            ISelfAssessmentService selfAssessmentService,
            IUnlockService unlockService,
            ILogger<LearningPortalController> logger,
            IConfiguration config,
            IFilteredApiHelperService filteredApiHelperService)
        {
            this.centresService = centresService;
            this.configService = configService;
            this.courseService = courseService;
            this.selfAssessmentService = selfAssessmentService;
            this.unlockService = unlockService;
            this.logger = logger;
            this.config = config;
            this.filteredApiHelperService = filteredApiHelperService;
        }

        public IActionResult AccessibilityHelp()
        {
            var accessibilityText = configService.GetConfigValue(ConfigService.AccessibilityHelpText);
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
            var termsText = configService.GetConfigValue(ConfigService.TermsText);
            if (termsText == null)
            {
                logger.LogError("Terms text from Config table is null");
                return StatusCode(500);
            }

            var model = new TermsViewModel(termsText);
            return View(model);
        }

        public IActionResult Error()
        {
            var model = GetErrorModel();
            Response.StatusCode = 500;
            return View("Error/UnknownError", model);
        }

        [Route("/LearningPortal/StatusCode/{code:int}")]
        [IgnoreAntiforgeryToken]
        public new IActionResult StatusCode(int code)
        {
            var model = GetErrorModel();
            Response.StatusCode = code;

            return code switch
            {
                404 => View("Error/PageNotFound", model),
                403 => View("Error/Forbidden", model),
                _ => View("Error/UnknownError", model)
            };
        }

        private int GetCandidateId()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.LearnCandidateId);
        }

        private int? GetCentreId()
        {
            return User.GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);
        }
        private string GetCandidateNumber()
        {
            string? sCandNum = User.GetCustomClaim(CustomClaimTypes.LearnCandidateNumber);
            if (sCandNum == null)
            {
                return "";
            }
            else
            {
                return sCandNum;
            }
        }
        private string GetCandidateForename()
        {
            string? sFName = User.GetCustomClaim(CustomClaimTypes.UserForename);
            if (sFName == null)
            {
                return "";
            }
            else
            {
                return sFName;
            }
        }
        private string GetCandidateSurname()
        {
            string? sLName = User.GetCustomClaim(CustomClaimTypes.UserSurname);
            if (sLName == null)
            {
                return "";
            }
            else
            {
                return sLName;
            }
        }
        private string? GetBannerText()
        {
            var centreId = GetCentreId();
            var bannerText = centreId == null
                ? null
                : centresService.GetBannerText(centreId.Value);
            return bannerText;
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
    }
}
