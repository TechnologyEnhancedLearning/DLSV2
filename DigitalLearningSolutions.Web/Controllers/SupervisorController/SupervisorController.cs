namespace DigitalLearningSolutions.Web.Controllers.SupervisorController
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserSupervisor)]
    public partial class SupervisorController : Controller
    {
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ISupervisorService supervisorService;
        private readonly IUserDataService userDataService;

        public SupervisorController(
            ISupervisorService supervisorService,
            IFrameworkNotificationService frameworkNotificationService,
            ISelfAssessmentService selfAssessmentService,
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            IUserDataService userDataService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.supervisorService = supervisorService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.selfAssessmentService = selfAssessmentService;
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.userDataService = userDataService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
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
