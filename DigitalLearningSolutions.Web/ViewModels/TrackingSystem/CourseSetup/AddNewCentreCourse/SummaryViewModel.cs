namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Linq;
    using DigitalLearningSolutions.Web.Models;

    public class SummaryViewModel
    {
        public SummaryViewModel() { }

        public SummaryViewModel(
            AddNewCentreCourseData data
        )
        {
            ApplicationName = data.SetCourseDetailsModel!.ApplicationName;
            CustomisationName = data.SetCourseDetailsModel.CustomisationName ?? string.Empty;
            Password = data.SetCourseDetailsModel.Password;
            NotificationEmails = data.SetCourseDetailsModel.NotificationEmails;
            AllowSelfEnrolment = data.SetCourseOptionsModel!.AllowSelfEnrolment;
            HideInLearningPortal = data.SetCourseOptionsModel.HideInLearningPortal;
            DiagAssess = data.Application!.DiagAssess;
            DiagnosticObjectiveSelection = data.SetCourseOptionsModel.DiagnosticObjectiveSelection;
            NoContent = data.SetSectionContentModels == null || !data.GetTutorialsFromSections().Any();
            IncludeAllSections = data.SetCourseContentModel!.IncludeAllSections;
            NumberOfLearning = GetNumberOfLearning(data);
            NumberOfDiagnostic = GetNumberOfDiagnostic(data);
        }

        public SummaryViewModel(
            string applicationName,
            string customisationName,
            string? password,
            string? notificationEmails,
            bool allowSelfEnrolment,
            bool hideInLearningPortal,
            bool diagAssess,
            bool diagnosticObjectiveSelection,
            bool noContent,
            bool includeAllSections,
            int numberOfLearning,
            int numberOfDiagnostic
        )
        {
            ApplicationName = applicationName;
            CustomisationName = customisationName;
            Password = password;
            NotificationEmails = notificationEmails;
            AllowSelfEnrolment = allowSelfEnrolment;
            HideInLearningPortal = hideInLearningPortal;
            DiagAssess = diagAssess;
            DiagnosticObjectiveSelection = diagnosticObjectiveSelection;
            NoContent = noContent;
            IncludeAllSections = includeAllSections;
            NumberOfLearning = numberOfLearning;
            NumberOfDiagnostic = numberOfDiagnostic;
        }

        public string ApplicationName { get; set; }
        public string CustomisationName { get; set; }
        public string? Password { get; set; }
        public string? NotificationEmails { get; set; }
        public bool AllowSelfEnrolment { get; set; }
        public bool HideInLearningPortal { get; set; }
        public bool DiagAssess { get; set; }
        public bool DiagnosticObjectiveSelection { get; set; }
        public bool NoContent { get; set; }
        public bool IncludeAllSections { get; set; }
        public int NumberOfLearning { get; set; }
        public int NumberOfDiagnostic { get; set; }

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
