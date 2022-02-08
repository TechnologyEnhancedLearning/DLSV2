namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableCourseViewModel : BaseFilterableViewModel
    {
        public SearchableCourseViewModel(CourseAssessmentDetails details, int groupId)
        {
            GroupId = groupId;
            CustomisationId = details.CustomisationId;
            CourseName = details.CourseName;
            Category = details.CategoryName;
            Topic = details.CourseTopic;
            Tags = FilterableTagHelper.GetCurrentTagsForCourse(details);
        }

        public int GroupId { get; set; }

        public int CustomisationId { get; set; }

        public string CourseName { get; }

        public string Category { get; }

        public string Topic { get; }
    }
}
