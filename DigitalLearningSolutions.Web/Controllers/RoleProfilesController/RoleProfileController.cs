namespace DigitalLearningSolutions.Web.Controllers.RoleProfilesController
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserFrameworksAdminOnly)]
    public partial class RoleProfileController : Controller
    {
        private readonly IRoleProfileService roleProfileService;
        private readonly ICommonService commonService;
        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly IConfigService configService;
        private readonly ILogger<RoleProfileController> logger;
        private readonly IConfiguration config;
        public RoleProfileController(
           IRoleProfileService roleProfileService,
           ICommonService commonService,
           IFrameworkNotificationService frameworkNotificationService,
          IConfigService configService,
           ILogger<RoleProfileController> logger,
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
        private string? GetUserEmail()
        {
            return User.GetEmail();
        }
        private string? GetUserFirstName()
        {
            return User.GetCustomClaim(CustomClaimTypes.UserForename);
        }
        private string? GetUserLastName()
        {
            return User.GetCustomClaim(CustomClaimTypes.UserSurname);
        }
    }
}
