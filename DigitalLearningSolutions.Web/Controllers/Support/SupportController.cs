namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Support;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.Support))]
    [TypeFilter(
        typeof(ValidateAllowedDlsSubApplication),
        Arguments = new object[]
            { new[] { nameof(DlsSubApplication.TrackingSystem), nameof(DlsSubApplication.Frameworks) } }
    )]
    public class SupportController : Controller
    {
        private readonly IConfiguration configuration;

        public SupportController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [Route("/{dlsSubApplication}/Support")]
        [Authorize(Policy = CustomPolicies.UserCentreAdminOrFrameworksAdmin)]
        public IActionResult Index(DlsSubApplication dlsSubApplication)
        {
            var model = new SupportViewModel(
                dlsSubApplication,
                SupportPage.Support,
                configuration.GetCurrentSystemBaseUrl()
            );
            return View("Support", model);
        }
    }
}
