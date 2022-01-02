﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;

    public class SummaryViewModel
    {
        // TODO: Create SummaryViewModel
        public SummaryViewModel() { }

        public SummaryViewModel(
            AddNewCentreCourseData data
        )
        {
            Application = data.SetCourseDetailsModel.ApplicationName;
            CustomisationName = data.SetCourseDetailsModel.CustomisationName ?? string.Empty;
            Password = data.SetCourseDetailsModel.Password;
            NotificationEmails = data.SetCourseDetailsModel.NotificationEmails;
            AllowSelfEnrolment = data.SetCourseOptionsModel.AllowSelfEnrolment;
            HideInLearningPortal = data.SetCourseOptionsModel.HideInLearningPortal;
            DiagnosticObjectiveSelection = data.SetCourseOptionsModel.DiagnosticObjectiveSelection;
            NumberOfLearning = GetNumberOfLearning(data.SetSectionContentModel);
            NumberOfDiagnostic = GetNumberOfDiagnostic(data.SetSectionContentModel);
        }

        public SummaryViewModel(
            string application,
            string customisationName,
            string? password,
            string? notificationEmails,
            bool allowSelfEnrolment,
            bool hideInLearningPortal,
            bool diagnosticObjectiveSelection,
            int numberOfLearning,
            int numberOfDiagnostic
        )
        {
            Application = application;
            CustomisationName = customisationName;
            Password = password;
            NotificationEmails = notificationEmails;
            AllowSelfEnrolment = allowSelfEnrolment;
            HideInLearningPortal = hideInLearningPortal;
            DiagnosticObjectiveSelection = diagnosticObjectiveSelection;
            NumberOfLearning = numberOfLearning;
            NumberOfDiagnostic = numberOfDiagnostic;
        }

        public string Application { get; set; }
        public string CustomisationName { get; set; }
        public string? Password { get; set; }
        public string? NotificationEmails { get; set; }
        public bool AllowSelfEnrolment { get; set; }
        public bool HideInLearningPortal { get; set; }
        public bool DiagnosticObjectiveSelection { get; set; }
        public int NumberOfLearning { get; set; }
        public int NumberOfDiagnostic { get; set; }

        private static int GetNumberOfLearning(SetSectionContentViewModel model)
        {
            var tutorials = model.GetTutorialsFromSections();

            return tutorials.Count(t => t.LearningEnabled);
        }

        private static int GetNumberOfDiagnostic(SetSectionContentViewModel model)
        {
            var tutorials = model.GetTutorialsFromSections();

            return tutorials.Count(t => t.LearningEnabled);
        }
    }
}
