namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
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
        private readonly IEvaluationSummaryService evaluationSummaryService;
        private readonly IUserDataService userDataService;

        public ReportsController(
            IActivityService activityService,
            IUserDataService userDataService,
            IEvaluationSummaryService evaluationSummaryService
        )
        {
            this.activityService = activityService;
            this.userDataService = userDataService;
            this.evaluationSummaryService = evaluationSummaryService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            var filterData = ActivityFilterData.GetDefaultFilterData(adminUser);

            Response.Cookies.SetReportsFilterCookie(filterData, DateTime.UtcNow);

            var activity = activityService.GetFilteredActivity(centreId, filterData);

            var (jobGroupName, courseCategoryName, courseName) = activityService.GetFilterNames(filterData);

            var filterModel = new ReportsFilterModel(
                filterData,
                jobGroupName,
                courseCategoryName,
                courseName,
                adminUser.CategoryId == 0
            );

            var evaluationSummaryModels = evaluationSummaryService.GetEvaluationSummaryModels(centreId, filterData);

            var model = new ReportsViewModel(activity, filterModel, evaluationSummaryModels);
            return View(model);
        }

        [Route("Data")]
        public IEnumerable<ActivityDataRowModel> GetGraphData()
        {
            var centreId = User.GetCentreId();
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            var filterData = Request.Cookies.ParseReportsFilterCookie(adminUser);

            var activity = activityService.GetFilteredActivity(centreId, filterData!);
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
