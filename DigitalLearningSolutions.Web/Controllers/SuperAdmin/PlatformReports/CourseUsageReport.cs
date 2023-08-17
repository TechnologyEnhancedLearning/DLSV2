using DigitalLearningSolutions.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.PlatformReports
{
    public partial class PlatformReportsController
    {
        [Route("Course/Usage")]
        public IActionResult CourseUsageReport()
        {
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminCourseUsageReportFilterCookie", null);
            Response.Cookies.SetReportsFilterCookie("SuperAdminCourseUsageReportFilterCookie", filterData, clockUtility.UtcNow);
            var activity = platformReportsService.GetFilteredCourseActivity(filterData);
            var (regionName, centreTypeName, centreName, jobGroupName, brandName, categoryName, courseName) = reportFilterService.GetSuperAdminCourseFilterNames(filterData);
            return View();
        }
    }
}
