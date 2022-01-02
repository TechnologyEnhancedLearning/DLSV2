namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SelectCourseViewModel : SelectCourseFormData
    {
        // TODO: Set up javascript filtering
        public SelectCourseViewModel() { }

        public SelectCourseViewModel(
            SelectCourseFormData formData,
            IEnumerable<SelectListItem> courseOptions
        ) : base(formData)
        {
            ApplicationId = formData.ApplicationId;
            CourseOptions = courseOptions;
        }

        public SelectCourseViewModel(
            IEnumerable<SelectListItem> courseOptions,
            int? applicationId = null
        )
        {
            CourseOptions = courseOptions;
            ApplicationId = applicationId;
        }

        public IEnumerable<SelectListItem> CourseOptions { get; set; }
    }
}
