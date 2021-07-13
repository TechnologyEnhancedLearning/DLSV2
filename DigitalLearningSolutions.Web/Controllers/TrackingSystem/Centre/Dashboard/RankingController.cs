﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
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
            period ??= Period.Fortnight.Id;

            var centreId = User.GetCentreId();

            var regions = regionDataService.GetRegionsAlphabetical();

            var centreRankings = centresService.GetCentresForCentreRankingPage(centreId, period!.Value, regionId);

            var model = new CentreRankingViewModel(centreRankings, centreId, regions, regionId, period);

            return View(model);
        }
    }
}
