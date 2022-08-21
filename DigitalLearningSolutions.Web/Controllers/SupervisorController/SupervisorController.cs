namespace DigitalLearningSolutions.Web.Controllers.SupervisorController
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
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
        private readonly IUserDataService userDataService;

        public SupervisorController(
           ISupervisorService supervisorService,
           ICommonService commonService,
           IFrameworkNotificationService frameworkNotificationService,
           ISelfAssessmentService selfAssessmentService,
           IFrameworkService frameworkService,
           IConfigDataService configDataService,
           ICentreRegistrationPromptsService centreRegistrationPromptsService,
           IUserDataService userDataService,
           ILogger<SupervisorController> logger,
           IConfiguration config,
           ISearchSortFilterPaginateService searchSortFilterPaginateService,
           IMultiPageFormService multiPageFormService
        )
        {
            this.supervisorService = supervisorService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.frameworkService = frameworkService;
            this.selfAssessmentService = selfAssessmentService;
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.userDataService = userDataService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.multiPageFormService = multiPageFormService;
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
            var adminEntity = userDataService.GetAdminById(adminId);
            return adminEntity!.EmailForCentreNotifications;
        }
    }
}
