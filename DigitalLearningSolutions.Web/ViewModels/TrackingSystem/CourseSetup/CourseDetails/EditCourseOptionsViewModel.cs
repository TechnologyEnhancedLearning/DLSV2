namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class EditCourseOptionsViewModel : EditCourseOptionsFormData
    {
        public EditCourseOptionsViewModel() { }

        public EditCourseOptionsViewModel(CourseOptions courseOptions, int customisationId)
        {
            Active = courseOptions.Active;
            AllowSelfEnrolment = courseOptions.SelfRegister;
            DiagnosticObjectiveSelection = courseOptions.DiagObjSelect;
            HideInLearningPortal = courseOptions.HideInLearnerPortal;
            CustomisationId = customisationId;
            SetUpCheckboxes(courseOptions.DiagAssess, true);
        }

        public int CustomisationId { get; set; }
    }
}
