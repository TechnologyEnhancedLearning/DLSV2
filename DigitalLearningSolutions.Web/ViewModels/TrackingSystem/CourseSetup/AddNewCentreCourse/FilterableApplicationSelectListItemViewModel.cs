namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class FilterableApplicationSelectListItemViewModel : SelectListItem
    {
        public FilterableApplicationSelectListItemViewModel(ApplicationDetails details, int? selectedApplicationId = null)
        {
            ApplicationId = details.ApplicationId;
            ApplicationName = details.ApplicationName;
            Category = details.CategoryName;
            Topic = details.CourseTopic;
            Selected = details.ApplicationId == selectedApplicationId;
        }

        public bool Selected { get; set; }

        public int ApplicationId { get; set; }

        public string ApplicationName { get; }

        public string Category { get; }

        public string CategoryFilter => nameof(CourseAssessmentDetails.CategoryName) + FilteringHelper.Separator +
                                        nameof(CourseAssessmentDetails.CategoryName) +
                                        FilteringHelper.Separator + Category;

        public string Topic { get; }

        public string TopicFilter => nameof(CourseAssessmentDetails.CourseTopic) + FilteringHelper.Separator +
                                     nameof(CourseAssessmentDetails.CourseTopic) +
                                     FilteringHelper.Separator + Topic;
    }
}
