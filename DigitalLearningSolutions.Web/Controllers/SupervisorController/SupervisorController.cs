namespace DigitalLearningSolutions.Web.Controllers.SupervisorController
{
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using GDS.MultiPageFormData;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserSupervisor)]
    public partial class SupervisorController : Controller
    {
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly IFrameworkService frameworkService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ISupervisorService supervisorService;
        private readonly IUserService userService;
        private readonly IRegistrationService registrationService;
        private readonly ICentresService centresService;
        private readonly IEmailGenerationService emailGenerationService;
        private readonly IEmailService emailService;
        private readonly ICandidateAssessmentDownloadFileService candidateAssessmentDownloadFileService;
        private readonly IClockUtility clockUtility;
        private readonly IPdfService pdfService;
        private readonly ICourseCategoriesService courseCategoriesService;

        public SupervisorController(
           ISupervisorService supervisorService,
           ICommonService commonService,
           IFrameworkNotificationService frameworkNotificationService,
           ISelfAssessmentService selfAssessmentService,
           IFrameworkService frameworkService,
           IConfigService configService,
           ICentreRegistrationPromptsService centreRegistrationPromptsService,
           IUserService userService,
           ILogger<SupervisorController> logger,
           IConfiguration config,
           ISearchSortFilterPaginateService searchSortFilterPaginateService,
           IMultiPageFormService multiPageFormService,
           IRegistrationService registrationService,
           ICentresService centresService,
           IEmailGenerationService emailGenerationService,
           IEmailService emailService,
           ICandidateAssessmentDownloadFileService candidateAssessmentDownloadFileService,
           IClockUtility clockUtility,
           IPdfService pdfService,
           ICourseCategoriesService courseCategoriesService
           )
        {
            this.supervisorService = supervisorService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.frameworkService = frameworkService;
            this.selfAssessmentService = selfAssessmentService;
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.userService = userService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.multiPageFormService = multiPageFormService;
            this.registrationService = registrationService;
            this.centresService = centresService;
            this.emailGenerationService = emailGenerationService;
            this.emailService = emailService;
            this.candidateAssessmentDownloadFileService = candidateAssessmentDownloadFileService;
            this.clockUtility = clockUtility;
            this.pdfService = pdfService;
            this.courseCategoriesService = courseCategoriesService;
        }

        private int GetCentreId()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserCentreId);
        }

        private int GetAdminId()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
        }

        private string GetUserEmail()
        {
            var adminId = GetAdminId();
            var adminEntity = userService.GetAdminById(adminId);
            return adminEntity!.EmailForCentreNotifications;
        }

        private string? GetBannerText()
        {
            var centreId = (int)User.GetCentreId();
            var bannerText = centresService.GetBannerText(centreId);
            return bannerText;
        }
    }
}
