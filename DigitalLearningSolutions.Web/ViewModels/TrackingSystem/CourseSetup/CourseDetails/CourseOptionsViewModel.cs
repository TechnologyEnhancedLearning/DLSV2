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
        }

        public bool Active { get; set; }
        public bool AllowSelfEnrolment { get; set; }
        public bool DiagnosticObjectiveSelection { get; set; }
        public bool HideInLearningPortal { get; set; }
    }
}
