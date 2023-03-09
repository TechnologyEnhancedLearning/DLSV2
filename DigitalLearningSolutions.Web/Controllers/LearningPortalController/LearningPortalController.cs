namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using GDS.MultiPageFormData;

    [Authorize(Policy = CustomPolicies.UserDelegateOnly)]
    public partial class LearningPortalController : Controller
    {
        private readonly IActionPlanService actionPlanService;
        private readonly ICentresDataService centresDataService;
        private readonly IConfiguration config;
        private readonly ICourseDataService courseDataService;
        private readonly IUserDataService userDataService;

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

        public LearningPortalController(
            ICentresDataService centresDataService,
            ICourseDataService courseDataService,
            IUserDataService userDataService,
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
            IClockUtility clockUtility
        )
        {
            this.centresDataService = centresDataService;
            this.courseDataService = courseDataService;
            this.userDataService = userDataService;
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
            var bannerText = centresDataService.GetBannerText(centreId);
            return bannerText;
        }
    }
}
