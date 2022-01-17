namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public class EditCourseOptionsFormData
    {
        public EditCourseOptionsFormData() { }

        public EditCourseOptionsFormData(
            bool allowSelfEnrolment,
            bool diagnosticObjectiveSelection,
            bool hideInLearningPortal
        )
        {
            AllowSelfEnrolment = allowSelfEnrolment;
            DiagnosticObjectiveSelection = diagnosticObjectiveSelection;
            HideInLearningPortal = hideInLearningPortal;
        }

        public List<CheckboxListItemViewModel> Checkboxes { get; set; }
        public bool AllowSelfEnrolment { get; set; }
        public bool DiagnosticObjectiveSelection { get; set; }
        public bool HideInLearningPortal { get; set; }

        public void SetUpCheckboxes(bool diagAssess)
        {
            Checkboxes = new List<CheckboxListItemViewModel>
            {
                CourseOptionsInputs.AllowSelfEnrolmentCheckbox,
                CourseOptionsInputs.HideInLearningPortalCheckbox,
            };
            if (diagAssess)
            {
                Checkboxes.Add(CourseOptionsInputs.DiagnosticObjectiveSelectionCheckbox);
            }
        }
    }
}
