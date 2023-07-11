﻿namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using System.Collections.Generic;
    using System.Linq;

    public interface IReportFilterService
    {
        (string jobGroupName, string courseCategoryName, string courseName) GetFilterNames(
           ActivityFilterData filterData
       );
        (string regionName, string centreName, string jobGroupName, string categoryName, string selfAssessmentName) GetNursingAssessmentCourseFilterNames(
            ActivityFilterData filterData
        );
        ReportsFilterOptions GetFilterOptions(int centreId, int? courseCategoryId);
        string GetCourseCategoryNameForActivityFilter(int? courseCategoryId);
    }
    public class ReportFilterService : IReportFilterService
    {
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly IRegionDataService regionDataService;
        private readonly ICentresDataService centresDataService;
        private readonly ICourseDataService courseDataService;
        private readonly ISelfAssessmentDataService selfAssessmentDataService;
        private readonly IJobGroupsDataService jobGroupsDataService;
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
        public (string regionName, string centreName, string jobGroupName, string categoryName, string selfAssessmentName) GetNursingAssessmentCourseFilterNames(
            ActivityFilterData filterData
        )
        {
            return (
                GetRegionNameForActivityFilter(filterData.RegionId),
                GetCentreNameForActivityFilter(filterData.CentreId),
                GetJobGroupNameForActivityFilter(filterData.JobGroupId),
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
        public string GetCourseCategoryNameForActivityFilter(int? courseCategoryId)
        {
            var courseCategoryName = courseCategoryId.HasValue
                ? courseCategoriesDataService.GetCourseCategoryName(courseCategoryId.Value)
                : "All";
            return courseCategoryName ?? "All";
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
