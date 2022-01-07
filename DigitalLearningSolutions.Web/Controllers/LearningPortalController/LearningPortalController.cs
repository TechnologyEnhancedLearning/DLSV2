namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserOnly)]
    public partial class LearningPortalController : Controller
    {
        private readonly IActionPlanService actionPlanService;
        private readonly ICentresDataService centresDataService;
        private readonly IConfiguration config;
        private readonly ICourseDataService courseDataService;

        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly ILogger<LearningPortalController> logger;
        private readonly INotificationService notificationService;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ISupervisorService supervisorService;

        public LearningPortalController(
            ICentresDataService centresDataService,
            ICourseDataService courseDataService,
            ISelfAssessmentService selfAssessmentService,
            ISupervisorService supervisorService,
            INotificationService notificationService,
            IFrameworkNotificationService frameworkNotificationService,
            ILogger<LearningPortalController> logger,
            IConfiguration config,
            IActionPlanService actionPlanService
        )
        {
            this.centresDataService = centresDataService;
            this.courseDataService = courseDataService;
            this.selfAssessmentService = selfAssessmentService;
            this.supervisorService = supervisorService;
            this.notificationService = notificationService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.logger = logger;
            this.config = config;
            this.actionPlanService = actionPlanService;
        }

        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        public IActionResult AccessDenied()
        {
            return View("~/Views/LearningSolutions/Error/AccessDenied.cshtml");
        }

        private int GetCandidateId()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.LearnCandidateId);
        }

        private string? GetBannerText()
        {
            var centreId = User.GetCentreId();
            var bannerText = centresDataService.GetBannerText(centreId);
            return bannerText;
        }
    }
}
