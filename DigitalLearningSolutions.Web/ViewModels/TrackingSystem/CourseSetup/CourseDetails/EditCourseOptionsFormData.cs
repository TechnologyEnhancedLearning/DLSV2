﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public class EditCourseOptionsFormData
    {
        public List<CheckboxListItemViewModel> Checkboxes { get; set; }
        public bool Active { get; set; }
        public bool AllowSelfEnrolment { get; set; }
        public bool DiagnosticObjectiveSelection { get; set; }
        public bool HideInLearningPortal { get; set; }

        public void SetUpCheckboxes(bool diagAssess)
        {
            Checkboxes = new List<CheckboxListItemViewModel>
            {
                CourseOptionsInputs.ActiveCheckbox,
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
