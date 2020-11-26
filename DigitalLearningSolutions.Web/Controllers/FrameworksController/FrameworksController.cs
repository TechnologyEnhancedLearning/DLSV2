namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using DigitalLearningSolutions.Web.ViewModels.LearningSolutions;

    [Authorize(Policy = CustomPolicies.UserOnly)]
    public partial class FrameworksController : Controller
    {
        private readonly IFrameworkService frameworkService;
        private readonly ICentresService centresService;
        private readonly IConfigService configService;
        private readonly ILogger<FrameworksController> logger;
        private readonly IConfiguration config;
        public FrameworksController(
            IFrameworkService frameworkService,
           ICentresService centresService,
           IConfigService configService,
            ILogger<FrameworksController> logger,
            IConfiguration config)
        {
            this.frameworkService = frameworkService;
            this.centresService = centresService;
            this.configService = configService;
            this.logger = logger;
            this.config = config;
        }
       
        private int? GetCentreId()
        {
            return User.GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);
        }
        private int GetAdminID()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
        }
        private bool? GetIsFrameworkDeveloper()
        {
            return User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkDeveloper);
        }      
    }
}
