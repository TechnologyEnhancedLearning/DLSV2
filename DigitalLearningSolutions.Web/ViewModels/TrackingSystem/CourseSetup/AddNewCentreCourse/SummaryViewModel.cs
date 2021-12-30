namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
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
            Application = data.SetCourseDetailsViewModel.ApplicationName;
            CustomisationName = data.SetCourseDetailsViewModel.CustomisationName ?? string.Empty;
            Password = data.SetCourseDetailsViewModel.Password;
            NotificationEmails = data.SetCourseDetailsViewModel.NotificationEmails;
            AllowSelfEnrolment = data.SetCourseOptionsViewModel.AllowSelfEnrolment;
            HideInLearningPortal = data.SetCourseOptionsViewModel.HideInLearningPortal;
            DiagnosticObjectiveSelection = data.SetCourseOptionsViewModel.DiagnosticObjectiveSelection;
            NumberOfLearning = GetNumberOfLearning(data.SetSectionContentViewModel);
            NumberOfDiagnostic = GetNumberOfDiagnostic(data.SetSectionContentViewModel);
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
            var tutorials = GetTutorialsFromSections(model.Sections);

            return tutorials.Count(t => t.LearningEnabled);
        }

        private static int GetNumberOfDiagnostic(SetSectionContentViewModel model)
        {
            var tutorials = GetTutorialsFromSections(model.Sections);

            return tutorials.Count(t => t.LearningEnabled);
        }

        private static IEnumerable<CourseTutorialViewModel> GetTutorialsFromSections(
            IEnumerable<SectionContentViewModel> sections
        )
        {
            var tutorials = new List<CourseTutorialViewModel>();
            foreach (var section in sections)
            {
                tutorials.AddRange(section.Tutorials);
            }

            return tutorials;
        }
    }
}
