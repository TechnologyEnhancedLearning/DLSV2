namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports;
    using Microsoft.AspNetCore.Mvc;
    public partial class PlatformReportsController
    {
        [Route("SelfAssessments/Independent")]
        public IActionResult IndependentSelfAssessmentsReport()
        {
            //Removing an old cookie if it exists because it may contain problematic options (filters that return too many rows):
            if (HttpContext.Request.Cookies.ContainsKey("SuperAdminDSATReportsFilterCookie"))
            {
                HttpContext.Response.Cookies.Delete("SuperAdminDSATReportsFilterCookie");
            }
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminIndependentSAReportsFilterCookie", null);
            Response.Cookies.SetReportsFilterCookie("SuperAdminIndependentSAReportsFilterCookie", filterData, clockUtility.UtcNow);
            var activity = platformReportsService.GetSelfAssessmentActivity(filterData, false);
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
                false
                );
            var model = new SelfAssessmentsReportViewModel(
                activity,
                selfAssessmentReportFilterModel,
                filterData.StartDate,
                filterData.EndDate ?? clockUtility.UtcToday,
                true,
                "All",
                false
                );

            return View("SelfAssessmentsReport", model);
        }

        [HttpGet]
        [Route("SelfAssessments/Independent/EditFilters")]
        public IActionResult IndependentSelfAssessmentsEditFilters()
        {
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminIndependentSAReportsFilterCookie", null);
            var filterOptions = GetDropdownValues(false);
            var dataStartDate = platformReportsService.GetSelfAssessmentActivityStartDate(false);
            var model = new SelfAssessmentsEditFiltersViewModel(
                filterData,
                null,
                filterOptions,
                dataStartDate,
                false
            );
            return View("SelfAssessmentsEditFilters", model);
        }

        [HttpPost]
        [Route("SelfAssessments/Independent/EditFilters")]
        public IActionResult IndependentSelfAssessmentsEditFilters(SelfAssessmentsEditFiltersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var filterOptions = GetDropdownValues(false);
                model.SetUpDropdowns(filterOptions, null);
                model.DataStart = platformReportsService.GetSelfAssessmentActivityStartDate(false);
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
            Response.Cookies.SetReportsFilterCookie("SuperAdminIndependentSAReportsFilterCookie", filterData, clockUtility.UtcNow);

            return RedirectToAction("IndependentSelfAssessmentsReport");
        }
    }
}
