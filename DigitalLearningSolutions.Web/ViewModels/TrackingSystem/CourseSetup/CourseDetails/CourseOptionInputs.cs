namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public static class CourseOptionsInputs
    {
        public static readonly CheckboxListItemViewModel AllowSelfEnrolmentCheckbox = new CheckboxListItemViewModel(
            nameof(EditCourseOptionsFormData.AllowSelfEnrolment),
            "Allow self-enrolment",
            "The course will be in the Available courses list of the Learning Portal for self-enrolment."
        );

        public static readonly CheckboxListItemViewModel HideInLearningPortalCheckbox = new CheckboxListItemViewModel(
            nameof(EditCourseOptionsFormData.HideInLearningPortal),
            "Hide in Learning Portal",
            "The course will not be visible to learners in the Learning Portal."
        );

        public static readonly CheckboxListItemViewModel DiagnosticObjectiveSelectionCheckbox = new CheckboxListItemViewModel(
            nameof(EditCourseOptionsFormData.DiagnosticObjectiveSelection),
            "Allow diagnostic objective selection",
            "Allow the learner to choose which objectives to be assessed against when starting a diagnostic assessment."
        );
    }
}
