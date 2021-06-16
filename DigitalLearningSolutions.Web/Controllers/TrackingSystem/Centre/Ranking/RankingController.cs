namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Ranking
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Ranking;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/Centre/Ranking")]
    public class RankingController : Controller
    {
        private readonly ICentresService centresService;

        public RankingController(ICentresService centresService)
        {
            this.centresService = centresService;
        }

        public IActionResult Index()
        {
            // TODO: HEEDLS-469 Populate these numbers from filters
            var centreRankings = centresService.GetCentreRanks(User.GetCentreId(), 14, -1);

            return View(new CentreRankingViewModel(centreRankings, User.GetCentreId()));
        }
    }
}
