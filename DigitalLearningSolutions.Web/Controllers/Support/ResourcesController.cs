namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Support.Resources;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;

    [Route("/{dlsSubApplication}/Resources")]
    [Authorize(Policy = CustomPolicies.UserCentreAdminOrFrameworksAdmin)]
    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.Support))]
    [TypeFilter(typeof(ValidateAllowedDlsSubApplication), Arguments = new object[] { new [] { nameof(DlsSubApplication.TrackingSystem), nameof(DlsSubApplication.Frameworks) } })]
    public class ResourcesController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IFeatureManager featureManager;
        private readonly IResourcesService resourcesService;

        public ResourcesController(
            IFeatureManager featureManager,
            IConfiguration configuration,
            IResourcesService resourcesService
        )
        {
            this.featureManager = featureManager;
            this.configuration = configuration;
            this.resourcesService = resourcesService;
        }

        public IActionResult Index(DlsSubApplication dlsSubApplication)
        {
            var model = new ResourcesViewModel(
                dlsSubApplication,
                SupportPage.Resources,
                configuration.GetCurrentSystemBaseUrl(),
                resourcesService.GetAllResources()
            );
            return View("Index", model);
        }
    }
}
