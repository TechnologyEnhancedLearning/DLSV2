namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SelectCourseViewModel
    {
        public SelectCourseViewModel() { }

        public SelectCourseViewModel(
            IEnumerable<SelectListItem> applicationOptions,
            IEnumerable<SelectListItem>? topicOptions,
            int? topicId
        )
        {
            ApplicationOptions = applicationOptions;
            TopicOptions = topicOptions;
            TopicId = topicId;
        }

        public SelectCourseViewModel(
            int applicationId,
            IEnumerable<SelectListItem> applicationOptions
        )
        {
            ApplicationId = applicationId;
            ApplicationOptions = applicationOptions;
        }

        [Required(ErrorMessage = "Select a course")]
        public int? ApplicationId { get; set; }

        public IEnumerable<SelectListItem> ApplicationOptions { get; set; }

        public IEnumerable<SelectListItem>? TopicOptions { get; set; }

        public int? TopicId { get; set; }
    }
}
