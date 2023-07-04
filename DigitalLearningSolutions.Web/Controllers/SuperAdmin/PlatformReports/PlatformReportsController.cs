namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]
    [Route("SuperAdmin/Reports")]
    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Reports))]
    public class PlatformReportsController : Controller
    {
        private readonly IPlatformReportsService platformReportsService;
        public PlatformReportsController(IPlatformReportsService platformReportsService)
        {
            this.platformReportsService = platformReportsService;
        }
        public IActionResult Index()
        {
            var model = new PlatformReportsViewModel
            {
                PlatformUsageSummary = platformReportsService.GetPlatformUsageSummary()
            };
            return View(model);
        }
    }
}
