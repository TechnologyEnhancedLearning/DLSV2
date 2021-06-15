namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Ranking
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Ranking;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/Centre/Ranking")]
    public class RankingController : Controller
    {
        public IActionResult Index()
        {
            return View(new CentreRankingViewModel());
        }
    }
}
