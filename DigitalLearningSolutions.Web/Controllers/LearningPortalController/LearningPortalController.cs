namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using GDS.MultiPageFormData;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserDelegateOnly)]
    public partial class LearningPortalController : Controller
    {
        private readonly IActionPlanService actionPlanService;
        private readonly ICentresService centresService;
        private readonly IConfiguration config;
        private readonly ICourseService courseService;
        private readonly IUserService userService;

        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly ILogger<LearningPortalController> logger;
        private readonly INotificationService notificationService;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ISupervisorService supervisorService;
        private readonly IFrameworkService frameworkService;
        private readonly ICandidateAssessmentDownloadFileService candidateAssessmentDownloadFileService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly IClockUtility clockUtility;
        private readonly IPdfService pdfService;
        public LearningPortalController(
            ICentresService centresService,
            ICourseService courseService,
            IUserService userService,
            ISelfAssessmentService selfAssessmentService,
            ISupervisorService supervisorService,
            IFrameworkService frameworkService,
            INotificationService notificationService,
            IFrameworkNotificationService frameworkNotificationService,
            ILogger<LearningPortalController> logger,
            IConfiguration config,
            IActionPlanService actionPlanService,
            ICandidateAssessmentDownloadFileService candidateAssessmentDownloadFileService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IMultiPageFormService multiPageFormService,
            IClockUtility clockUtility,
            IPdfService pdfService
        )
        {
            this.centresService = centresService;
            this.courseService = courseService;
            this.userService = userService;
            this.selfAssessmentService = selfAssessmentService;
            this.supervisorService = supervisorService;
            this.frameworkService = frameworkService;
            this.notificationService = notificationService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.logger = logger;
            this.config = config;
            this.actionPlanService = actionPlanService;
            this.candidateAssessmentDownloadFileService = candidateAssessmentDownloadFileService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.multiPageFormService = multiPageFormService;
            this.clockUtility = clockUtility;
            this.pdfService = pdfService;
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
            var centreId = User.GetCentreIdKnownNotNull();
            var bannerText = centresService.GetBannerText(centreId);
            return bannerText;
        }
    }
}
