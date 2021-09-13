﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
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
        private readonly IActivityDataService activityDataService;
        private readonly IActivityService activityService;
        private readonly IUserDataService userDataService;

        public ReportsController(
            IActivityService activityService,
            IActivityDataService activityDataService,
            IUserDataService userDataService
        )
        {
            this.activityService = activityService;
            this.activityDataService = activityDataService;
            this.userDataService = userDataService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            var filterData = Request.Cookies.RetrieveFilterDataFromCookie(adminUser);

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

            var model = new ReportsViewModel(activity, filterModel);
            return View(model);
        }

        [NoCaching]
        [Route("Data")]
        public IEnumerable<ActivityDataRowModel> GetGraphData()
        {
            var centreId = User.GetCentreId();
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            var filterData = Request.Cookies.RetrieveFilterDataFromCookie(adminUser);

            var activity = activityService.GetFilteredActivity(centreId, filterData!);
            return activity.Select(
                p => new ActivityDataRowModel(p, DateHelper.GetFormatStringForGraphLabel(p.DateInformation.Interval))
            );
        }

        [HttpGet]
        [Route("EditFilters")]
        public IActionResult EditFilters()
        {
            var centreId = User.GetCentreId();
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            var filterData = Request.Cookies.RetrieveFilterDataFromCookie(adminUser);

            var (jobGroups, courseCategories, courses) =
                activityService.GetFilterOptions(
                    centreId,
                    adminUser.CategoryId == 0 ? (int?)null : adminUser.CategoryId
                );

            var dataStartDate = activityDataService.GetStartOfActivityForCentre(centreId);

            var model = new EditFiltersViewModel(
                filterData,
                adminUser.CategoryId,
                jobGroups,
                courseCategories,
                courses,
                dataStartDate
            );
            return View(model);
        }

        [HttpPost]
        [Route("EditFilters")]
        public IActionResult EditFilters(EditFiltersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var startDate = new DateTime(model.StartYear!.Value, model.StartMonth!.Value, model.StartDay!.Value);
            var endDate = model.EndDate
                ? new DateTime(model.EndYear!.Value, model.EndMonth!.Value, model.EndDay!.Value)
                : (DateTime?)null;

            var filterData = new ActivityFilterData(
                startDate,
                endDate,
                model.JobGroupId,
                model.CourseCategoryId,
                model.CustomisationId,
                model.ReportInterval
            );

            Response.Cookies.SetReportsFilterCookie(filterData, DateTime.UtcNow);

            return RedirectToAction("Index");
        }
    }
}
