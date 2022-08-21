namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]
    [Route("SuperAdmin/Centres")]
    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Centres))]
    public class CentresController : Controller
    {
        private readonly ICentresService centresService;

        public CentresController(ICentresService centresService)
        {
            this.centresService = centresService;
        }

        public IActionResult Index()
        {
            var centres = centresService.GetAllCentreSummariesForSuperAdmin();
            var viewModel = new CentresViewModel(centres);

            return View(viewModel);
        }
    }
}
