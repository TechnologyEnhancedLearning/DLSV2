namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public class CourseOptionsInputs
    {
        public static CheckboxListItemViewModel ActiveCheckbox = new CheckboxListItemViewModel(
            nameof(EditCourseOptionsFormData.Active),
            "Active",
            "Active courses are open to new enrolments. Inactive courses are not."
        );

        public static CheckboxListItemViewModel AllowSelfEnrolmentCheckbox = new CheckboxListItemViewModel(
            nameof(EditCourseOptionsFormData.AllowSelfEnrolment),
            "Allow self-enrolment",
            "The course will be in the Available courses list of the Learning Portal for self-enrolment."
        );

        public static CheckboxListItemViewModel HideInLearningPortalCheckbox = new CheckboxListItemViewModel(
            nameof(EditCourseOptionsFormData.HideInLearningPortal),
            "Hide in Learning Portal",
            "The course will not be visible to learners in the Learning Portal."
        );

        public static CheckboxListItemViewModel DiagnosticObjectiveSelectionCheckbox = new CheckboxListItemViewModel(
            nameof(EditCourseOptionsFormData.DiagnosticObjectiveSelection),
            "Allow diagnostic objective selection",
            "Allow the learner to choose which objectives to be assessed against when starting a diagnostic assessment."
        );
    }
}
