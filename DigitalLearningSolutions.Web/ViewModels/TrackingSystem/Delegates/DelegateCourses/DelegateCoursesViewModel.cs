namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using System.Collections.Generic;
    using System.Linq;

    public class DelegateCoursesViewModel : BaseSearchablePageViewModel<CourseStatistics>
    {
        public DelegateCoursesViewModel(
            SearchSortFilterPaginationResult<CourseStatistics> result,
            IEnumerable<FilterModel> availableFilters,
            string courseCategoryName
        ) : base(
            result,
            true,
            availableFilters,
            "Search by activity name"
        )
        {
            UpdateCourseActiveFlags(result);

            Courses = result.ItemsToDisplay.Select<BaseSearchableItem, SearchableDelegateCourseStatisticsViewModel>(
                activity =>
                {
                    return activity switch
                    {
                        CourseStatisticsWithAdminFieldResponseCounts currentCourse => new SearchableDelegateCourseStatisticsViewModel(currentCourse),
                        _ => new SearchableDelegateAssessmentStatisticsViewModel((DelegateAssessmentStatistics)activity),
                    };
                }
             );

            CourseCategoryName = courseCategoryName;
        }

        private static void UpdateCourseActiveFlags(SearchSortFilterPaginationResult<CourseStatistics> result)
        {
            foreach (var course in result.ItemsToDisplay)
            {
                if (course is CourseStatisticsWithAdminFieldResponseCounts)
                {
                    CourseStatisticsWithAdminFieldResponseCounts courseStatisticsWithAdminFieldResponseCounts = (CourseStatisticsWithAdminFieldResponseCounts)course;

                    if (courseStatisticsWithAdminFieldResponseCounts.Active && !courseStatisticsWithAdminFieldResponseCounts.Archived)
                    {
                        courseStatisticsWithAdminFieldResponseCounts.Active = true;
                    }
                    else
                    {
                        courseStatisticsWithAdminFieldResponseCounts.Active = false;
                    }

                    if (courseStatisticsWithAdminFieldResponseCounts.Archived)
                    {
                        courseStatisticsWithAdminFieldResponseCounts.Archived = true;
                    }
                    else
                    {
                        courseStatisticsWithAdminFieldResponseCounts.Archived = false;
                    }
                }
            }
        }

        public IEnumerable<SearchableDelegateCourseStatisticsViewModel> Courses { get; set; }
        public string CourseCategoryName { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.ActivityName,
            CourseSortByOptions.Completed,
            CourseSortByOptions.InProgress,
        };

        public override bool NoDataFound => !Courses.Any() && NoSearchOrFilter;
    }
}
