namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public class EditCourseOptionsFormData
    {
        public EditCourseOptionsFormData() { }

        public EditCourseOptionsFormData(CourseOptionsTempData tempData)
        {
            Active = tempData.Active;
            AllowSelfEnrolment = tempData.AllowSelfEnrolment;
            DiagnosticObjectiveSelection = tempData.DiagnosticObjectiveSelection;
            HideInLearningPortal = tempData.HideInLearningPortal;
        }

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
        public bool Active { get; set; }
        public bool AllowSelfEnrolment { get; set; }
        public bool DiagnosticObjectiveSelection { get; set; }
        public bool HideInLearningPortal { get; set; }

        public void SetUpCheckboxes(bool diagAssess, bool showActiveCheckbox = false)
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

            if (showActiveCheckbox)
            {
                Checkboxes = Checkboxes.Prepend(CourseOptionsInputs.ActiveCheckbox).ToList();
            }
        }
    }
}
