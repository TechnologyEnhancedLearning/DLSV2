﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public class EditCourseOptionsViewModel
    {
        public readonly List<CheckboxListItemViewModel> Checkboxes = new List<CheckboxListItemViewModel>
        {
            new CheckboxListItemViewModel(
                nameof(Active),
                "Active",
                "Active courses are open to new enrolments. Inactive courses are not"
            ),
            new CheckboxListItemViewModel(
                nameof(AllowSelfEnrolment),
                "Allow self-enrolment",
                "The course will be in the Available courses list of the Learning Portal for self-enrolment"
            ),
            new CheckboxListItemViewModel(
                nameof(HideInLearningPortal),
                "Hide in Learning Portal",
                "The course will not be visible to learners in the Learning Portal"
            )
        };

        public EditCourseOptionsViewModel(CourseDetails courseDetails)
        {
            Active = courseDetails.Active;
            AllowSelfEnrolment = courseDetails.SelfRegister;
            DiagnosticObjectiveSelection = courseDetails.DiagObjSelect;
            HideInLearningPortal = courseDetails.HideInLearnerPortal;
            CustomisationId = courseDetails.CustomisationId;
            DiagAssess = courseDetails.DiagAssess;

            if (DiagAssess)
            {
                Checkboxes.Add(
                    new CheckboxListItemViewModel(
                        nameof(DiagnosticObjectiveSelection),
                        "Allow diagnostic objective selection",
                        "Allow the learner to choose which objectives to be assessed against when starting a diagnostic assessment"
                    )
                );
            }
        }

        public EditCourseOptionsViewModel() { }
        public int CustomisationId { get; set; }
        public bool Active { get; set; }
        public bool AllowSelfEnrolment { get; set; }
        public bool DiagnosticObjectiveSelection { get; set; }
        public bool HideInLearningPortal { get; set; }
        public bool DiagAssess { get; set; }
    }
}
