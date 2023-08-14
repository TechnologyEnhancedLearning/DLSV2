namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using System.Collections.Generic;
    using System.Linq;

    public interface IReportFilterService
    {
        (string jobGroupName, string courseCategoryName, string courseName) GetFilterNames(
           ActivityFilterData filterData
       );
        (string regionName, string centreTypeName, string centreName, string jobGroupName, string brandName, string categoryName, string selfAssessmentName) GetSelfAssessmentFilterNames(
            ActivityFilterData filterData
        );
        ReportsFilterOptions GetFilterOptions(int centreId, int? courseCategoryId);
        string GetCourseCategoryNameForActivityFilter(int? courseCategoryId);
        SelfAssessmentReportsFilterOptions GetSelfAssessmentFilterOptions(bool supervised);
    }
    public class ReportFilterService : IReportFilterService
    {
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly IRegionDataService regionDataService;
        private readonly ICentresDataService centresDataService;
        private readonly ICourseDataService courseDataService;
        private readonly ISelfAssessmentDataService selfAssessmentDataService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly ICommonService commonService;
        public ReportFilterService(
           ICourseCategoriesDataService courseCategoriesDataService,
           IRegionDataService regionDataService,
           ICentresDataService centresDataService,
           ICourseDataService courseDataService,
           ISelfAssessmentDataService selfAssessmentDataService,
           IJobGroupsDataService jobGroupsDataService,
           ICommonService commonService
           )
        {
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.regionDataService = regionDataService;
            this.centresDataService = centresDataService;
            this.courseDataService = courseDataService;
            this.selfAssessmentDataService = selfAssessmentDataService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.commonService = commonService;
        }
        public (string jobGroupName, string courseCategoryName, string courseName) GetFilterNames(
            ActivityFilterData filterData
        )
        {
            return (GetJobGroupNameForActivityFilter(filterData.JobGroupId),
                GetCourseCategoryNameForActivityFilter(filterData.CourseCategoryId),
                GetCourseNameForActivityFilter(filterData.CustomisationId));
        }
        public (string regionName, string centreName, string jobGroupName, string categoryName, string courseName) GetSuperAdminCourseFilterNames(
            ActivityFilterData filterData
        )
        {
            return (
                GetRegionNameForActivityFilter(filterData.RegionId),
                GetCentreNameForActivityFilter(filterData.CentreId),
                GetJobGroupNameForActivityFilter(filterData.JobGroupId),
                GetCourseCategoryNameForActivityFilter(filterData.CourseCategoryId),
                GetCourseNameForActivityFilter(filterData.CustomisationId));
        }
        public (string regionName, string centreTypeName, string centreName, string jobGroupName, string brandName, string categoryName, string selfAssessmentName) GetSelfAssessmentFilterNames(
            ActivityFilterData filterData
        )
        {
            return (
                GetRegionNameForActivityFilter(filterData.RegionId),
                GetCentreTypeNameForActivityFilter(filterData.CentreTypeId),
                GetCentreNameForActivityFilter(filterData.CentreId),
                GetJobGroupNameForActivityFilter(filterData.JobGroupId),
                GetBrandNameForActivityFilter(filterData.BrandId),
                GetCourseCategoryNameForActivityFilter(filterData.CourseCategoryId),
                GetSelfAssessmentNameForActivityFilter(filterData.SelfAssessmentId)
                );
        }
        public ReportsFilterOptions GetFilterOptions(int centreId, int? courseCategoryId)
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var courseCategories = courseCategoriesDataService
                .GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                .Select(cc => (cc.CourseCategoryID, cc.CategoryName));

            var availableCourses = courseDataService
                .GetCoursesAvailableToCentreByCategory(centreId, courseCategoryId);
            var historicalCourses = courseDataService
                .GetCoursesEverUsedAtCentreByCategory(centreId, courseCategoryId);

            var courses = availableCourses.Union(historicalCourses, new CourseEqualityComparer())
                .OrderByDescending(c => c.Active)
                .ThenBy(c => c.CourseName)
                .Select(c => (c.CustomisationId, c.CourseNameWithInactiveFlag));

            return new ReportsFilterOptions(jobGroups, courseCategories, courses);
        }
        public SelfAssessmentReportsFilterOptions GetSelfAssessmentFilterOptions(bool supervised)
        {
            var centreTypes = commonService.GetSelfAssessmentCentreTypes(supervised);
            var regions = commonService.GetSelfAssessmentRegions(supervised);
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var centres = commonService.GetSelfAssessmentCentres(supervised);
            var categories = commonService.GetSelfAssessmentCategories(supervised);
            var brands = commonService.GetSelfAssessmentBrands(supervised);
            var selfAssessments = commonService.GetSelfAssessments(supervised);
            return new SelfAssessmentReportsFilterOptions(centreTypes, regions, centres, jobGroups,  brands, categories, selfAssessments);
        }
        public string GetCourseCategoryNameForActivityFilter(int? courseCategoryId)
        {
            var courseCategoryName = courseCategoryId.HasValue
                ? courseCategoriesDataService.GetCourseCategoryName(courseCategoryId.Value)
                : "All";
            return courseCategoryName ?? "All";
        }
        public string GetBrandNameForActivityFilter(int? brandId)
        {
            var brandName = brandId.HasValue
                ? commonService.GetBrandNameById(brandId.Value)
                : "All";
            return brandName ?? "All";
        }
        private string GetJobGroupNameForActivityFilter(int? jobGroupId)
        {
            var jobGroupName = jobGroupId.HasValue
                ? jobGroupsDataService.GetJobGroupName(jobGroupId.Value)
                : "All";
            return jobGroupName ?? "All";
        }

        private string GetCourseNameForActivityFilter(int? courseId)
        {
            var courseNames = courseId.HasValue
                ? courseDataService.GetCourseNameAndApplication(courseId.Value)
                : null;
            return courseNames?.CourseName ?? "All";
        }
        private string GetRegionNameForActivityFilter(int? regionId)
        {
            var regionName = regionId.HasValue
                ? regionDataService.GetRegionName(regionId.Value) : null;
            return regionName ?? "All";
        }
        private string GetCentreNameForActivityFilter(int? centreId)
        {
            var centreName = centreId.HasValue
                ? centresDataService.GetCentreName(centreId.Value) : null;
            return centreName ?? "All";
        }
        private string GetCentreTypeNameForActivityFilter(int? centreTypeId)
        {
            var centreTypeName = centreTypeId.HasValue
                ? commonService.GetCentreTypeNameById(centreTypeId.Value) : null;
            return centreTypeName ?? "All";
        }
        private string GetSelfAssessmentNameForActivityFilter(int? selfAssessmentId)
        {
            var selfAssessment = selfAssessmentId.HasValue
                ? selfAssessmentDataService.GetSelfAssessmentNameById(selfAssessmentId.Value) : null;
            return selfAssessment ?? "All";
        }
        private static int GetFirstMonthOfQuarter(int quarter)
        {
            return quarter * 3 - 2;
        }
        private class CourseEqualityComparer : IEqualityComparer<Course>
        {
            public bool Equals(Course? x, Course? y)
            {
                return x?.CustomisationId == y?.CustomisationId;
            }

            public int GetHashCode(Course obj)
            {
                return obj.CustomisationId;
            }
        }
    }
}
