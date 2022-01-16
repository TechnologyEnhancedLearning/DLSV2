namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Web.Models;
    using DocumentFormat.OpenXml.Drawing;

    public class SummaryViewModel
    {
        public SummaryViewModel() { }

        public SummaryViewModel(
            AddNewCentreCourseData data
        )
        {
            Application = data.SetCourseDetailsModel!.ApplicationName;
            CustomisationName = data.SetCourseDetailsModel.CustomisationName ?? string.Empty;
            Password = data.SetCourseDetailsModel.Password;
            NotificationEmails = data.SetCourseDetailsModel.NotificationEmails;
            AllowSelfEnrolment = data.SetCourseOptionsModel!.AllowSelfEnrolment;
            HideInLearningPortal = data.SetCourseOptionsModel.HideInLearningPortal;
            DiagAssess = data.Application!.DiagAssess;
            DiagnosticObjectiveSelection = data.SetCourseOptionsModel.DiagnosticObjectiveSelection;
            IncludeAllSections = data.SetCourseContentModel!.IncludeAllSections;
            NumberOfLearning = GetNumberOfLearning(data);
            NumberOfDiagnostic = GetNumberOfDiagnostic(data);
            NumberOfTutorials = data.GetTutorialsFromSections().Count();
        }

        public SummaryViewModel(
            string application,
            string customisationName,
            string? password,
            string? notificationEmails,
            bool active,
            bool allowSelfEnrolment,
            bool hideInLearningPortal,
            bool diagAssess,
            bool diagnosticObjectiveSelection,
            bool includeAllSections,
            int numberOfLearning,
            int numberOfDiagnostic,
            int numberOfTutorials
        )
        {
            Application = application;
            CustomisationName = customisationName;
            Password = password;
            NotificationEmails = notificationEmails;
            AllowSelfEnrolment = allowSelfEnrolment;
            HideInLearningPortal = hideInLearningPortal;
            DiagAssess = diagAssess;
            DiagnosticObjectiveSelection = diagnosticObjectiveSelection;
            IncludeAllSections = includeAllSections;
            NumberOfLearning = numberOfLearning;
            NumberOfDiagnostic = numberOfDiagnostic;
            NumberOfTutorials = numberOfTutorials;
        }

        public string Application { get; set; }
        public string CustomisationName { get; set; }
        public string? Password { get; set; }
        public string? NotificationEmails { get; set; }
        public bool AllowSelfEnrolment { get; set; }
        public bool HideInLearningPortal { get; set; }
        public bool DiagAssess { get; set; }
        public bool DiagnosticObjectiveSelection { get; set; }
        public bool IncludeAllSections { get; set; }
        public int NumberOfLearning { get; set; }
        public int NumberOfDiagnostic { get; set; }
        public int NumberOfTutorials { get; set; }

        private static int GetNumberOfLearning(AddNewCentreCourseData data)
        {
            var tutorials = data.GetTutorialsFromSections();
            return tutorials.Count(t => t.LearningEnabled);
        }

        private static int GetNumberOfDiagnostic(AddNewCentreCourseData data)
        {
            var tutorials = data.GetTutorialsFromSections();
            return tutorials.Count(t => t.DiagnosticEnabled);
        }
    }
}
