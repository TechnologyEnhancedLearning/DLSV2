using DigitalLearningSolutions.Data.Models.Courses;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

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
