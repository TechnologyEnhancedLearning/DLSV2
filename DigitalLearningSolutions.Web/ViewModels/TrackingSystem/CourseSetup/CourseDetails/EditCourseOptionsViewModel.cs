namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;

    public class EditCourseOptionsViewModel : EditCourseOptionsFormData
    {
        public EditCourseOptionsViewModel(CourseOptions courseOptions, int customisationId)
        {
            Active = courseOptions.Active;
            AllowSelfEnrolment = courseOptions.SelfRegister;
            DiagnosticObjectiveSelection = courseOptions.DiagObjSelect;
            HideInLearningPortal = courseOptions.HideInLearnerPortal;
            CustomisationId = customisationId;

            if (courseOptions.DiagAssess)
            {
                Checkboxes.Add(
                    new CheckboxListItemViewModel(
                        nameof(DiagnosticObjectiveSelection),
                        "Allow diagnostic objective selection",
                        "Allow the learner to choose which objectives to be assessed against when starting a diagnostic assessment."
                    )
                );
            }
        }

        public EditCourseOptionsViewModel() { }

        public int CustomisationId { get; set; }
    }
}
