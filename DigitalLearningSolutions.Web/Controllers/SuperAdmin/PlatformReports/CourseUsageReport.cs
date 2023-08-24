using DigitalLearningSolutions.Data.Models.PlatformReports;
using DigitalLearningSolutions.Data.Models.TrackingSystem;
using DigitalLearningSolutions.Web.Attributes;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Common;
using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.PlatformReports
{
    public partial class PlatformReportsController
    {
        [Route("CourseUsage")]
        public IActionResult CourseUsageReport()
        {
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminCourseUsageReportFilterCookie", null);
            Response.Cookies.SetReportsFilterCookie("SuperAdminCourseUsageReportFilterCookie", filterData, clockUtility.UtcNow);
            var activity = platformReportsService.GetFilteredCourseActivity(filterData);
            var (regionName, centreTypeName, centreName, jobGroupName, brandName, categoryName, courseName) = reportFilterService.GetSuperAdminCourseFilterNames(filterData);
            var courseUsageReportFilterModel = new CourseUsageReportFilterModel(
                filterData,
                regionName,
                centreTypeName,
                centreName,
                jobGroupName,
                brandName,
                categoryName,
                courseName,
                true
                );
            var model = new CourseUsageReportViewModel(
                activity,
                courseUsageReportFilterModel,
                filterData.StartDate,
                filterData.EndDate ?? clockUtility.UtcToday,
                true,
                "All"
                );
            return View("CourseUsageReport", model);
        }
        [HttpGet]
        [Route("CourseUsage/EditFilters")]
        public IActionResult CourseUsageEditFilters()
        {
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminCourseUsageReportFilterCookie", null);
            var filterOptions = GetCourseFilterDropdownValues();
            var dataStartDate = platformReportsService.GetStartOfCourseActivity();
            var model = new CourseUsageEditFiltersViewModel(
                filterData,
                null,
                filterOptions,
                dataStartDate
            );
            return View("CourseUsageEditFilters", model);
        }
        private CourseUsageReportFilterOptions GetCourseFilterDropdownValues()
        {
            return reportFilterService.GetCourseUsageFilterOptions();
        }
        [HttpPost]
        [Route("CourseUsage/EditFilters")]
        public IActionResult CourseUsageEditFilters(CourseUsageEditFiltersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var filterOptions = GetDropdownValues(false);
                model.DataStart = platformReportsService.GetStartOfCourseActivity();
                return View("CourseUsageEditFilters", model);
            }

            var filterData = new ActivityFilterData(
                model.GetValidatedStartDate(),
                model.GetValidatedEndDate(),
                model.JobGroupId,
                model.CategoryId,
                null,
                model.ApplicationId,
                model.RegionId,
                model.CentreId,
                null,
                model.CentreTypeId,
                model.BrandId,
                model.CoreContent,
                model.FilterType,
                model.ReportInterval
            );
            Response.Cookies.SetReportsFilterCookie("SuperAdminCourseUsageReportFilterCookie", filterData, clockUtility.UtcNow);

            return RedirectToAction("CourseUsageReport");
        }

        [NoCaching]
        [Route("CourseUsage/Data")]
        public IEnumerable<ActivityDataRowModel> GetCourseGraphData(string selfAssessmentType)
        {
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminCourseUsageReportFilterCookie", null);
            var activity = platformReportsService.GetFilteredCourseActivity(filterData);
            return activity.Select(
                p => new ActivityDataRowModel(p, DateHelper.GetFormatStringForGraphLabel(p.DateInformation.Interval))
            );
        }
    }
}
