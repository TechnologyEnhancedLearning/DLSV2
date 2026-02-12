namespace DigitalLearningSolutions.Web.Controllers.CompetencyAssessmentsController
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using GDS.MultiPageFormData;
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
        private readonly ISelfAssessmentNotificationService selfAssessmentNotificationService;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ICentresService centresService;
        private readonly ICentreSelfAssessmentsService centreSelfAssessmentsService;
        private readonly IEnrolService enrolService;
        private readonly ILogger<CompetencyAssessmentsController> logger;
        private readonly IConfiguration config;
        private readonly IMultiPageFormService multiPageFormService;
        public CompetencyAssessmentsController(
           ICompetencyAssessmentService competencyAssessmentService,
           IFrameworkService frameworkService,
           ICommonService commonService,
           IFrameworkNotificationService frameworkNotificationService,
           ISelfAssessmentNotificationService selfAssessmentNotificationService,
           ISelfAssessmentService selfAssessmentService,
           ICentresService centresService,
           ICentreSelfAssessmentsService centreSelfAssessmentsService,
           IEnrolService enrolService,
           ILogger<CompetencyAssessmentsController> logger,
           IConfiguration config,
           IMultiPageFormService multiPageFormService)
        {
            this.competencyAssessmentService = competencyAssessmentService;
            this.frameworkService = frameworkService;
            this.commonService = commonService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.selfAssessmentNotificationService = selfAssessmentNotificationService;
            this.selfAssessmentService = selfAssessmentService;
            this.centresService = centresService;
            this.centreSelfAssessmentsService = centreSelfAssessmentsService;
            this.enrolService = enrolService;
            this.logger = logger;
            this.config = config;
            this.multiPageFormService = multiPageFormService;
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
