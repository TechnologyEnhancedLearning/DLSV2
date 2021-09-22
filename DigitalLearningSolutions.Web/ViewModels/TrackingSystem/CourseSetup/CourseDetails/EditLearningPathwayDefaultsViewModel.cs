namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class EditLearningPathwayDefaultsViewModel
    {
        public EditLearningPathwayDefaultsViewModel() { }

        public EditLearningPathwayDefaultsViewModel(CourseDetails courseDetails)
        {
            CustomisationId = courseDetails.CustomisationId;
            CompleteWithinMonths = courseDetails.CompleteWithinMonths;
            ValidityMonths = courseDetails.ValidityMonths;
            Mandatory = courseDetails.Mandatory;
            AutoRefresh = courseDetails.AutoRefresh;
        }

        public EditLearningPathwayDefaultsViewModel(
            int customisationId,
            int completeWithinMonths,
            int validityMonths,
            bool mandatory,
            bool autoRefresh
        )
        {
            CustomisationId = customisationId;
            CompleteWithinMonths = completeWithinMonths;
            ValidityMonths = validityMonths;
            Mandatory = mandatory;
            AutoRefresh = autoRefresh;
        }

        public int CustomisationId { get; set; }

        [Required(ErrorMessage = "Select a field name")]
        [Range(0, 48, ErrorMessage = "Value must be between 0 and 48 months.")]
        public int CompleteWithinMonths { get; set; }

        [Required(ErrorMessage = "Select a field name")]
        [Range(0, 48, ErrorMessage = "Value must be between 0 and 48 months.")]
        public int ValidityMonths { get; set; }

        public bool Mandatory { get; set; }

        public bool AutoRefresh { get; set; }
    }
}
