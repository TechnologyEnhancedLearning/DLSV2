namespace DigitalLearningSolutions.Web.Controllers.SupervisorController
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserSupervisor)]
    public partial class SupervisorController : Controller
    {
        private readonly IRoleProfileService roleProfileService;
        private readonly ICommonService commonService;
        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly IConfigService configService;
        private readonly ILogger<SupervisorController> logger;
        private readonly IConfiguration config;
        public SupervisorController(
           IRoleProfileService roleProfileService,
           ICommonService commonService,
           IFrameworkNotificationService frameworkNotificationService,
          IConfigService configService,
           ILogger<SupervisorController> logger,
           IConfiguration config)
        {
            this.roleProfileService = roleProfileService;
            this.commonService = commonService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.configService = configService;
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
    }
}
