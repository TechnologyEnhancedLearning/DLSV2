namespace DigitalLearningSolutions.Web.Controllers.SupervisorController
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserSupervisor)]
    public partial class SupervisorController : Controller
    {
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly ICommonService commonService;
        private readonly IConfiguration config;
        private readonly IConfigDataService configDataService;
        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly ILogger<SupervisorController> logger;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ISupervisorService supervisorService;
        private readonly IUserDataService userDataService;

        public SupervisorController(
            ISupervisorService supervisorService,
            ICommonService commonService,
            IFrameworkNotificationService frameworkNotificationService,
            ISelfAssessmentService selfAssessmentService,
            IConfigDataService configDataService,
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            IUserDataService userDataService,
            ILogger<SupervisorController> logger,
            IConfiguration config,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.supervisorService = supervisorService;
            this.commonService = commonService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.selfAssessmentService = selfAssessmentService;
            this.configDataService = configDataService;
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.userDataService = userDataService;
            this.logger = logger;
            this.config = config;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        private int GetCentreId()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserCentreId);
        }

        private int GetAdminID()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
        }

        private string GetUserEmail()
        {
            var adminId = GetAdminID();
            var adminEntity = userDataService.GetAdminById(adminId);
            return adminEntity!.UserCentreDetails?.Email ?? adminEntity.UserAccount.PrimaryEmail;
        }
    }
}
