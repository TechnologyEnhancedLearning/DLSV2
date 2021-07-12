namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Ranking;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/Centre/Ranking")]
    public class RankingController : Controller
    {
        private readonly ICentresService centresService;

        public RankingController(ICentresService centresService)
        {
            this.centresService = centresService;
        }

        public IActionResult Index(int? regionId = null)
        {
            var centreId = User.GetCentreId();
            // TODO: HEEDLS-469 Populate these numbers from filters
            var centreRankings = centresService.GetCentresForCentreRankingPage(centreId, 14, null);

            return View(new CentreRankingViewModel(centreRankings, centreId));
        }
    }
}
