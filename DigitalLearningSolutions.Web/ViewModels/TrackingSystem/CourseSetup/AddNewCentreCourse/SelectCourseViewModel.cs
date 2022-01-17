namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SelectCourseViewModel
    {
        public SelectCourseViewModel() { }

        public SelectCourseViewModel(
            IEnumerable<SelectListItem> courseOptions,
            IEnumerable<SelectListItem>? topicOptions,
            int? topicId
        )
        {
            CourseOptions = courseOptions;
            TopicOptions = topicOptions;
            TopicId = topicId;
        }

        [Required(ErrorMessage = "Select a course")]
        public int? ApplicationId { get; set; }

        public IEnumerable<SelectListItem> CourseOptions { get; set; }

        public IEnumerable<SelectListItem>? TopicOptions { get; set; }

        public int? TopicId { get; set; }
    }
}
