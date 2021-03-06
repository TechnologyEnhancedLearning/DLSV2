﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Reports
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/Centre/Reports")]
    public class ReportsController : Controller
    {
        private readonly IActivityService activityService;

        public ReportsController(IActivityService activityService)
        {
            this.activityService = activityService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var monthsOfActivity = activityService.GetRecentActivity(centreId);
            var model = new ReportsViewModel(monthsOfActivity);
            return View(model);
        }

        [Route("/TrackingSystem/Centre/Reports/Data")]
        public IEnumerable<ActivityDataRowModel> GetRecentData()
        {
            var centreId = User.GetCentreId();
            var monthsOfActivity = activityService.GetRecentActivity(centreId);
            return monthsOfActivity.Select(m => new ActivityDataRowModel(m, "MMM yyyy"));
        }
    }
}
