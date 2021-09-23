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
            CompleteWithinMonths = courseDetails.CompleteWithinMonths.ToString();
            ValidityMonths = courseDetails.ValidityMonths.ToString();
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
            CompleteWithinMonths = completeWithinMonths.ToString();
            ValidityMonths = validityMonths.ToString();
            Mandatory = mandatory;
            AutoRefresh = autoRefresh;
        }

        public int CustomisationId { get; set; }

        public string? CompleteWithinMonths { get; set; }

        public string? ValidityMonths { get; set; }

        public bool Mandatory { get; set; }

        public bool AutoRefresh { get; set; }
    }
}
