namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AddCourseToGroupCoursesViewModel : BaseSearchablePageViewModel<CourseAssessmentDetails>
    {
        public AddCourseToGroupCoursesViewModel(
            SearchSortFilterPaginationResult<CourseAssessmentDetails> result,
            IEnumerable<FilterModel> availableFilters,
            int? adminCategoryFilter,
            int groupId,
            string groupName
        ) : base(
            result,
            true,
            availableFilters,
            "Search courses"
        )
        {
            GroupId = groupId;
            GroupName = groupName;
            AdminCategoryFilter = adminCategoryFilter;
            Courses = result.ItemsToDisplay.Select(c => new SearchableCourseViewModel(c, groupId));
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public int? AdminCategoryFilter { get; set; }

        public IEnumerable<SearchableCourseViewModel> Courses { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();

        public override bool NoDataFound => !Courses.Any() && NoSearchOrFilter;
    }
}
