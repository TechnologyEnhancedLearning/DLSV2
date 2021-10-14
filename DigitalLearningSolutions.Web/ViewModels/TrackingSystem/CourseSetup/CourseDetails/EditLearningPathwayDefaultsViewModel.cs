namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Attributes;

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
            string? completeWithinMonths,
            string? validityMonths,
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

        [WholeNumberWithinInclusiveRange(0, 48, "Enter a whole number from 0 to 48")]
        public string? CompleteWithinMonths { get; set; }

        [WholeNumberWithinInclusiveRange(0, 48, "Enter a whole number from 0 to 48")]
        public string? ValidityMonths { get; set; }

        public bool Mandatory { get; set; }

        public bool AutoRefresh { get; set; }
    }
}
