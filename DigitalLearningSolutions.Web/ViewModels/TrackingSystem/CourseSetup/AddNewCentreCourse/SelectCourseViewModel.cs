namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SelectCourseViewModel : BaseJavaScriptFilterableViewModel
    {
        public SelectCourseViewModel() { }

        public SelectCourseViewModel(
            IEnumerable<string> topics,
            int? topicId,
            int? customisationId
        )
        {
            TopicId = topicId;
            CustomisationId = customisationId;
        }

        public int? TopicId { get; set; }

        [Required(ErrorMessage = "Select a course")]
        public int? CustomisationId { get; set; }
    }
}
