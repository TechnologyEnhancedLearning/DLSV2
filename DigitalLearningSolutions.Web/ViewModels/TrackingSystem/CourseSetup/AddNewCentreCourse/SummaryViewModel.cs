namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse;

    public class SummaryViewModel
    {
        public SummaryViewModel() { }

        public SummaryViewModel(
            AddNewCentreCourseData data
        )
        {
            ApplicationName = data.CourseDetailsData!.ApplicationName;
            CustomisationName = data.CourseDetailsData.CustomisationName ?? string.Empty;
            Password = data.CourseDetailsData.Password;
            NotificationEmails = data.CourseDetailsData.NotificationEmails;
            PostLearningAssessment = data.CourseDetailsData.IsAssessed;
            RequiredLearningPercentage = data.CourseDetailsData.TutCompletionThreshold;
            RequiredDiagnosticPercentage = data.CourseDetailsData.DiagCompletionThreshold;
            AllowSelfEnrolment = data.CourseOptionsData!.AllowSelfEnrolment;
            HideInLearningPortal = data.CourseOptionsData.HideInLearningPortal;
            DiagAssess = data.Application!.DiagAssess;
            DiagnosticObjectiveSelection = data.CourseOptionsData.DiagnosticObjectiveSelection;
            NoContent = data.SectionContentData == null || !data.GetTutorialsFromSections().Any();
            IncludeAllSections = !NoContent && data.CourseContentData!.IncludeAllSections;
            NumberOfLearning = NoContent ? 0 : GetNumberOfLearning(data);
            NumberOfDiagnostic = NoContent ? 0 : GetNumberOfDiagnostic(data);
        }

        public string ApplicationName { get; set; }
        public string CustomisationName { get; set; }
        public string? Password { get; set; }
        public string? NotificationEmails { get; set; }
        public bool PostLearningAssessment { get; set; }
        public string? RequiredLearningPercentage { get; set; }
        public string? RequiredDiagnosticPercentage { get; set; }
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
