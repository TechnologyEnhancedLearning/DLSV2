namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
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

            var filterData = new ActivityFilterData(
                DateTime.Today.AddYears(-1),
                DateTime.UtcNow,
                null,
                null,
                null,
                ReportInterval.Months
            );

            var activity = activityService.GetFilteredActivity(centreId, filterData);
            var filterModel = new ActivityFilterModel
            {
                CourseCategoryName = "All categories",
                CustomisationName = "Customisation",
                JobGroupName = "Job group",
                ReportIntervalName = nameof(ReportInterval.Months),
                StartDate = DateTime.Now.AddYears(-1),
                EndDate = DateTime.Now
            };

            var model = new ReportsViewModel(activity, filterModel);
            return View(model);
        }

        [Route("Data")]
        public IEnumerable<ActivityDataRowModel> GetRecentData()
        {
            var centreId = User.GetCentreId();
            var filterData = new ActivityFilterData(
                DateTime.UtcNow.Date.AddYears(-1),
                DateTime.UtcNow,
                null,
                null,
                null,
                ReportInterval.Months
            );

            var activity = activityService.GetFilteredActivity(centreId, filterData);
            return activity.Select(
                p => new ActivityDataRowModel(p, DateHelper.GetFormatStringForGraphLabel(p.DateInformation.Interval))
            );
        }

        // [HttpGet]
        // [Route("EditFilters")]
        // public IActionResult EditFilters()
        // {
        //     return View();
        // }
    }
}
