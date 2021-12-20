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
            IEnumerable<SelectListItem> courseOptions,
            int applicationId
        ) : base(formData)
        {
            ApplicationId = applicationId;
            CourseOptions = courseOptions;
        }

        public SelectCourseViewModel(
            IEnumerable<SelectListItem> courseOptions
        )
        {
            CourseOptions = courseOptions;
        }

        public IEnumerable<SelectListItem> CourseOptions { get; set; }

        public ApplicationDetails Application { get; set; }
    }
}
