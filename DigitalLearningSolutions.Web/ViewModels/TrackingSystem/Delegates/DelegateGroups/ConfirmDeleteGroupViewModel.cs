namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Web.Attributes;

    public class ConfirmDeleteGroupViewModel
    {
        public string GroupLabel { get; set; }
        public int DelegateCount { get; set; }
        public int CourseCount { get; set; }

        [BooleanMustBeTrue(ErrorMessage = "Confirm you wish to delete this group")]
        public bool Confirm { get; set; }

        public bool DeleteEnrolments { get; set; }
        public int? ReturnPage { get; set; }
    }
}
