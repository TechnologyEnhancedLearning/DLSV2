namespace DigitalLearningSolutions.Web.Controllers.RoleProfilesController
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserFrameworksAdminOnly)]
    public partial class RoleProfilesController : Controller
    {
        private readonly IRoleProfileService roleProfileService;
        private readonly ICommonService commonService;
        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly IConfigDataService configDataService;
        private readonly ILogger<RoleProfilesController> logger;
        private readonly IConfiguration config;
        public RoleProfilesController(
           IRoleProfileService roleProfileService,
           ICommonService commonService,
           IFrameworkNotificationService frameworkNotificationService,
          IConfigDataService configDataService,
           ILogger<RoleProfilesController> logger,
           IConfiguration config)
        {
            this.roleProfileService = roleProfileService;
            this.commonService = commonService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.configDataService = configDataService;
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
    }
}
