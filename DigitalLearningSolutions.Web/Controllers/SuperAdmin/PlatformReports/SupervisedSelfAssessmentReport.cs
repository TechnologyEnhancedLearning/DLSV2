﻿namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports;
    using DocumentFormat.OpenXml.InkML;
    using Microsoft.AspNetCore.Mvc;

    public partial class PlatformReportsController
    {
        [Route("SelfAssessments/Supervised")]
        public IActionResult SupervisedSelfAssessmentsReport()
        {
            //Removing an old cookie if it exists because it may contain problematic options (filters that return too many rows):
            if (HttpContext.Request.Cookies.ContainsKey("SuperAdminReportsFilterCookie"))
            {
                HttpContext.Response.Cookies.Delete("SuperAdminReportsFilterCookie");
            }
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminSupervisedSAReportsFilterCookie", null);
            Response.Cookies.SetReportsFilterCookie("SuperAdminSupervisedSAReportsFilterCookie", filterData, clockUtility.UtcNow);
            var activity = platformReportsService.GetSelfAssessmentActivity(filterData, true);
            var (regionName, centreTypeName, centreName, jobGroupName, brandName, categoryName, selfAssessmentName) = reportFilterService.GetSelfAssessmentFilterNames(filterData);
            var selfAssessmentReportFilterModel = new SelfAssessmentReportFilterModel(
                filterData,
                regionName,
                centreTypeName,
                centreName,
                jobGroupName,
                brandName,
                categoryName,
                selfAssessmentName,
                true,
                true
                );
            var model = new SelfAssessmentsReportViewModel(
                activity,
                selfAssessmentReportFilterModel,
                filterData.StartDate,
                filterData.EndDate ?? clockUtility.UtcToday,
                true,
                "All",
                true
                );

            return View("SelfAssessmentsReport", model);
        }



        [HttpGet]
        [Route("SelfAssessments/Supervised/EditFilters")]
        public IActionResult SupervisedSelfAssessmentsEditFilters()
        {
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminSupervisedSAReportsFilterCookie", null);
            var filterOptions = GetDropdownValues(true);
            var dataStartDate = platformReportsService.GetSelfAssessmentActivityStartDate(true);
            var model = new SelfAssessmentsEditFiltersViewModel(
                filterData,
                null,
                filterOptions,
                dataStartDate,
                true
            );
            return View("SelfAssessmentsEditFilters", model);
        }

        [HttpPost]
        [Route("SelfAssessments/Supervised/EditFilters")]
        public IActionResult SupervisedSelfAssessmentsEditFilters(SelfAssessmentsEditFiltersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var filterOptions = GetDropdownValues(true);
                model.SetUpDropdowns(filterOptions, null);
                model.DataStart = platformReportsService.GetSelfAssessmentActivityStartDate(true);
                model.Supervised = true;
                return View("SelfAssessmentsEditFilters", model);
            }

            var filterData = new ActivityFilterData(
                model.GetValidatedStartDate(),
                model.GetValidatedEndDate(),
                model.JobGroupId,
                model.CategoryId,
                null,
                null,
                model.RegionId,
                model.CentreId,
                model.SelfAssessmentId,
                model.CentreTypeId,
                model.BrandId,
                null,
                model.FilterType,
                model.ReportInterval
            );
            Response.Cookies.SetReportsFilterCookie("SuperAdminSupervisedSAReportsFilterCookie", filterData, clockUtility.UtcNow);
            return RedirectToAction("SupervisedSelfAssessmentsReport");
        }
    }
}
