namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserOnly)]
    public partial class LearningPortalController : Controller
    {
        private readonly ICentresService centresService;
        private readonly ICourseService courseService;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly INotificationService notificationService;
        private readonly ILogger<LearningPortalController> logger;
        private readonly IConfiguration config;
        private readonly IFilteredApiHelperService filteredApiHelperService;

        public LearningPortalController(
            ICentresService centresService,
            ICourseService courseService,
            ISelfAssessmentService selfAssessmentService,
            INotificationService notificationService,
            ILogger<LearningPortalController> logger,
            IConfiguration config,
            IFilteredApiHelperService filteredApiHelperService)
        {
            this.centresService = centresService;
            this.courseService = courseService;
            this.selfAssessmentService = selfAssessmentService;
            this.notificationService = notificationService;
            this.logger = logger;
            this.config = config;
            this.filteredApiHelperService = filteredApiHelperService;
        }

        private string GetCandidateNumber()
        {
            return User.GetCustomClaim(CustomClaimTypes.LearnCandidateNumber) ?? "";
        }

        private string? GetBannerText()
        {
            var centreId = User.GetCentreId();
            var bannerText = centresService.GetBannerText(centreId);
            return bannerText;
        }
    }
}
