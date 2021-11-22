namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditAutoRefreshOptionsViewModel : EditAutoRefreshOptionsFormData
    {
        public EditAutoRefreshOptionsViewModel() { }

        public EditAutoRefreshOptionsViewModel(
            EditAutoRefreshOptionsFormData formData,
            int customisationId,
            IEnumerable<SelectListItem> courseOptions
        ) : base(formData)
        {
            CustomisationId = customisationId;
            CourseOptions = courseOptions;
        }

        public EditAutoRefreshOptionsViewModel(
            CourseDetails courseDetails,
            int customisationId,
            IEnumerable<SelectListItem> courseOptions
        ) : base(courseDetails)
        {
            CustomisationId = customisationId;
            CourseOptions = courseOptions;
        }

        public int CustomisationId { get; set; }

        public IEnumerable<SelectListItem> CourseOptions { get; set; }
    }
}
