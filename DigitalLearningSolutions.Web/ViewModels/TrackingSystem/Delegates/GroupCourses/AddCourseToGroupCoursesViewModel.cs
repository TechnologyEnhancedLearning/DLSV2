namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;

    public class AddCourseToGroupCoursesViewModel
    {
        public AddCourseToGroupCoursesViewModel(
            IEnumerable<CourseAssessmentDetails> courses,
            int groupId,
            string groupName
        )
        {
            GroupId = groupId;
            GroupName = groupName;

            var sortedCourses = GenericSortingHelper.SortAllItems(
                courses.AsQueryable(),
                nameof(CourseAssessmentDetails.CourseName),
                BaseSearchablePageViewModel.Ascending
            );

            var coursesToShow = sortedCourses.Take(10);

            Courses = coursesToShow.Select(c => new SearchableCourseViewModel(c, groupId));
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public IEnumerable<SearchableCourseViewModel> Courses { get; set; }
    }
}
