namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Ranking;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Centre))]
    [Route("/TrackingSystem/Centre/Ranking")]
    public class RankingController : Controller
    {
        private readonly ICentresService centresService;
        private readonly IRegionService regionService;

        public RankingController(
            ICentresService centresService,
            IRegionService regionService
        )
        {
            this.centresService = centresService;
            this.regionService = regionService;
        }

        public IActionResult Index(int? regionId = null, Period? period = null)
        {
            period ??= Period.Fortnight;

            var centreId = User.GetCentreIdKnownNotNull();

            var regions = regionService.GetRegionsAlphabetical();

            var centreRankings = centresService.GetCentresForCentreRankingPage(centreId, period.Days, regionId);

            var model = new CentreRankingViewModel(centreRankings, centreId, regions, regionId, period);

            return View(model);
        }
    }
}
