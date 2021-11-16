namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class CourseOptionsViewModel
    {
        public CourseOptionsViewModel(CourseDetails courseDetails)
        {
            Active = courseDetails.Active;
            AllowSelfEnrolment = courseDetails.SelfRegister;
            DiagnosticObjectiveSelection = courseDetails.DiagObjSelect;
            HideInLearningPortal = courseDetails.HideInLearnerPortal;
            CustomisationId = courseDetails.CustomisationId;
        }

        public int CustomisationId { get; set; }
        public bool Active { get; set; }
        public bool AllowSelfEnrolment { get; set; }
        public bool DiagnosticObjectiveSelection { get; set; }
        public bool HideInLearningPortal { get; set; }
    }
}
