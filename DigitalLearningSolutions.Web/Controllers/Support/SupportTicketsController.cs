namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Support;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;

    [Route("/{dlsSubApplication}/Support/Tickets")]
    [Authorize(Policy = CustomPolicies.UserCentreAdminOrFrameworksAdmin)]
    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.Support))]
    [TypeFilter(
        typeof(ValidateAllowedDlsSubApplication),
        Arguments = new object[]
            { new[] { nameof(DlsSubApplication.TrackingSystem), nameof(DlsSubApplication.Frameworks) } }
    )]
    public class SupportTicketsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IFeatureManager featureManager;

        public SupportTicketsController(IFeatureManager featureManager, IConfiguration configuration)
        {
            this.featureManager = featureManager;
            this.configuration = configuration;
        }

        public IActionResult Index(DlsSubApplication dlsSubApplication)
        {
            var model = new SupportTicketsViewModel(
                dlsSubApplication,
                SupportPage.SupportTickets,
                configuration.GetCurrentSystemBaseUrl()
            );
            return View("Tickets", model);
        }
    }
}
