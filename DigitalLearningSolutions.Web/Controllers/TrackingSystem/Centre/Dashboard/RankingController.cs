namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
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
        private static readonly (int, string) Week = (7, "Week");
        private static readonly (int, string) Fortnight = (14, "Fortnight");
        private static readonly (int, string) Month = (30, "Month");
        private static readonly (int, string) Quarter = (91, "Quarter");
        private static readonly (int, string) Year = (365, "Year");

        private static readonly IEnumerable<(int, string)> Periods = new[]
        {
            Week,
            Fortnight,
            Month,
            Quarter,
            Year
        };

        private readonly ICentresService centresService;
        private readonly IRegionDataService regionDataService;

        public RankingController(
            ICentresService centresService,
            IRegionDataService regionDataService
        )
        {
            this.centresService = centresService;
            this.regionDataService = regionDataService;
        }

        public IActionResult Index(int? regionId = null, int? period = null)
        {
            period ??= Fortnight.Item1;

            var centreId = User.GetCentreId();

            var regions = regionDataService.GetRegionsAlphabetical();
            ViewBag.RegionOptions =
                SelectListHelper.MapOptionsToSelectListItems(regions, regionId);
            ViewBag.PeriodOptions = SelectListHelper.MapOptionsToSelectListItems(Periods, period);
            var centreRankings = centresService.GetCentresForCentreRankingPage(centreId, period!.Value, regionId);

            return View(new CentreRankingViewModel(centreRankings, centreId, regionId, period));
        }
    }
}
