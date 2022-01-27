namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AddCourseToGroupCoursesViewModel : BaseSearchablePageViewModel
    {
        public AddCourseToGroupCoursesViewModel(
            IEnumerable<CourseAssessmentDetails> courses,
            IEnumerable<string> categories,
            IEnumerable<string> topics,
            int? adminCategoryFilter,
            int groupId,
            string groupName,
            string? searchString,
            string? filterBy,
            int page
        ) : base(searchString, page, true, filterBy: filterBy, searchLabel: "Search courses")
        {
            GroupId = groupId;
            GroupName = groupName;
            AdminCategoryFilter = adminCategoryFilter;

            var courseList = courses.ToList();
            var searchedCourses = GenericSearchHelper.SearchItems(courseList, SearchString);
            var filteredCourses = FilteringHelper.FilterItems(searchedCourses.AsQueryable(), filterBy).ToList();
            var sortedCourses = GenericSortingHelper.SortAllItems(
                filteredCourses.AsQueryable(),
                nameof(CourseAssessmentDetails.CourseName),
                Ascending
            ).ToList();
            MatchingSearchResults = sortedCourses.Count;
            SetTotalPages();
            var coursesToShow = GetItemsOnCurrentPage(sortedCourses);

            Courses = coursesToShow.Select(c => new SearchableCourseViewModel(c, groupId));

            Filters = adminCategoryFilter == null
                ? AddCourseToGroupViewModelFilterOptions.GetAllCategoriesFilters(categories, topics)
                : AddCourseToGroupViewModelFilterOptions.GetSingleCategoryFilters(courseList);
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public int? AdminCategoryFilter { get; set; }

        public IEnumerable<SearchableCourseViewModel> Courses { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name,
        };

        public override bool NoDataFound => !Courses.Any() && NoSearchOrFilter;
    }
}
