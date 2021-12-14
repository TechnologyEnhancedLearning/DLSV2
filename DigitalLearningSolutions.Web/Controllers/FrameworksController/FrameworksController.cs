namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserFrameworksAdminOnly)]
    public partial class FrameworksController : Controller
    {
        private readonly IFrameworkService frameworkService;
        private readonly ICommonService commonService;
        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly IConfigService configService;
        private readonly ILogger<FrameworksController> logger;
        private readonly IConfiguration config;
        private readonly IImportCompetenciesFromFileService importCompetenciesFromFileService;
        private readonly ICompetencyLearningResourcesDataService competencyLearningResourcesDataService;

        public FrameworksController(
            IFrameworkService frameworkService,
            ICommonService commonService,
            IFrameworkNotificationService frameworkNotificationService,
            IConfigService configService,
            ILogger<FrameworksController> logger,
            IConfiguration config,
            IImportCompetenciesFromFileService importCompetenciesFromFileService,
            ICompetencyLearningResourcesDataService competencyLearningResourcesDataService
        )
        {
            this.frameworkService = frameworkService;
            this.commonService = commonService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.configService = configService;
            this.logger = logger;
            this.config = config;
            this.importCompetenciesFromFileService = importCompetenciesFromFileService;
            this.competencyLearningResourcesDataService = competencyLearningResourcesDataService;
        }

        private int? GetCentreId()
        {
            return User.GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);
        }

        private int GetAdminID()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
        }

        private bool GetIsFrameworkDeveloper()
        {
            var isFrameworkDeveloper = User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkDeveloper);
            return isFrameworkDeveloper != null && (bool)isFrameworkDeveloper;
        }

        private bool GetIsFrameworkContributor()
        {
            var isFrameworkContributor = User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkContributor);
            return isFrameworkContributor != null && (bool)isFrameworkContributor;
        }

        private string? GetUserFirstName()
        {
            return User.GetCustomClaim(CustomClaimTypes.UserForename);
        }

        private string? GetUserLastName()
        {
            return User.GetCustomClaim(CustomClaimTypes.UserSurname);
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
