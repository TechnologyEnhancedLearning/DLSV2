namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse;

    public class SummaryViewModel
    {
        public SummaryViewModel() { }

        public SummaryViewModel(
            AddNewCentreCourseTempData tempData
        )
        {
            ApplicationName = tempData.CourseDetailsData!.ApplicationName;
            CustomisationName = tempData.CourseDetailsData.CustomisationName ?? string.Empty;
            Password = tempData.CourseDetailsData.Password;
            NotificationEmails = tempData.CourseDetailsData.NotificationEmails;
            PostLearningAssessment = tempData.CourseDetailsData.IsAssessed;
            RequiredLearningPercentage = tempData.CourseDetailsData.TutCompletionThreshold;
            RequiredDiagnosticPercentage = tempData.CourseDetailsData.DiagCompletionThreshold;
            AllowSelfEnrolment = tempData.CourseOptionsData!.AllowSelfEnrolment;
            HideInLearningPortal = tempData.CourseOptionsData.HideInLearningPortal;
            DiagAssess = tempData.Application!.DiagAssess;
            DiagnosticObjectiveSelection = tempData.CourseOptionsData.DiagnosticObjectiveSelection;
            NoContent = tempData.SectionContentData == null || !tempData.GetTutorialsFromSections().Any();
            IncludeAllSections = !NoContent && tempData.CourseContentData!.IncludeAllSections;
            NumberOfLearning = NoContent ? 0 : GetNumberOfLearning(tempData);
            NumberOfDiagnostic = NoContent ? 0 : GetNumberOfDiagnostic(tempData);
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

        private static int GetNumberOfLearning(AddNewCentreCourseTempData tempData)
        {
            var tutorials = tempData.GetTutorialsFromSections();
            return tutorials.Count(t => t.LearningEnabled);
        }

        private static int GetNumberOfDiagnostic(AddNewCentreCourseTempData tempData)
        {
            var tutorials = tempData.GetTutorialsFromSections();
            return tutorials.Count(t => t.DiagnosticEnabled);
        }
    }
}
