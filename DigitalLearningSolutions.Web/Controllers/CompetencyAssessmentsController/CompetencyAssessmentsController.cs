namespace DigitalLearningSolutions.Web.Controllers.CompetencyAssessmentsController
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserFrameworksAdminOnly)]
    public partial class CompetencyAssessmentsController : Controller
    {
        private readonly ICompetencyAssessmentService competencyAssessmentService;
        private readonly IFrameworkService frameworkService;
        private readonly ICommonService commonService;
        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly ILogger<CompetencyAssessmentsController> logger;
        private readonly IConfiguration config;
        public CompetencyAssessmentsController(
           ICompetencyAssessmentService competencyAssessmentService,
           IFrameworkService frameworkService,
           ICommonService commonService,
           IFrameworkNotificationService frameworkNotificationService,
           ILogger<CompetencyAssessmentsController> logger,
           IConfiguration config)
        {
            this.competencyAssessmentService = competencyAssessmentService;
            this.frameworkService = frameworkService;
            this.commonService = commonService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.logger = logger;
            this.config = config;
        }
        public IActionResult Index()
        {
            return View();
        }
        private int? GetCentreId()
        {
            return User.GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);
        }
        private int GetAdminID()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
        }
        private bool GetIsWorkforceManager()
        {
            var isWorkforceManager = User.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceManager);
            return isWorkforceManager != null && (bool)isWorkforceManager;
        }
        private bool GetIsWorkforceContributor()
        {
            var isWorkforceContributor = User.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceContributor);
            return isWorkforceContributor != null && (bool)isWorkforceContributor;
        }
        private bool GetIsFrameworkDeveloper()
        {
            var isFrameworkDeveloper = User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkDeveloper);
            return isFrameworkDeveloper != null && (bool)isFrameworkDeveloper;
        }
        private bool GetIsFrameworkContributor()
        {
            var isFrameworkContributor = User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkContributor);
            return isFrameworkContributor != null && (bool)isFrameworkContributor;
        }
    }
}
