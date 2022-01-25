namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AddCourseToGroupCoursesAllCoursesViewModel : BaseJavaScriptFilterableViewModel
    {
        public AddCourseToGroupCoursesAllCoursesViewModel(
            IEnumerable<CourseAssessmentDetails> courses,
            IEnumerable<string> categories,
            IEnumerable<string> topics,
            int groupId
        )
        {
            Courses = courses.Select(c => new SearchableCourseViewModel(c, groupId));

            Filters = new[]
            {
                new FilterViewModel(
                    nameof(CourseAssessmentDetails.CategoryName),
                    "Category",
                    AddCourseToGroupViewModelFilterOptions.GetCategoryOptions(categories)
                ),
                new FilterViewModel(
                    nameof(CourseAssessmentDetails.CourseTopic),
                    "Topic",
                    AddCourseToGroupViewModelFilterOptions.GetTopicOptions(topics)
                ),
            }.SelectAppliedFilterViewModels();
        }

        public IEnumerable<SearchableCourseViewModel> Courses { get; set; }
    }
}
