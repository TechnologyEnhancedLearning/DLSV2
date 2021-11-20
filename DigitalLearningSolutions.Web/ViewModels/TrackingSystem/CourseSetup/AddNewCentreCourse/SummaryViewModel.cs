namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    public class SummaryViewModel
    {
        // TODO: Create SummaryViewModel
        public SummaryViewModel() { }

        public SummaryViewModel(
            string customisationName,
            string customisationNameSuffix,
            string? password,
            string? notificationEmails,
            bool allowSelfEnrolment,
            bool hideInLearningPortal,
            bool diagnosticObjectiveSelection,
            bool numberOfLearning,
            bool numberOfDiagnostic
        )
        {
            CustomisationName = customisationName;
            CustomisationNameSuffix = customisationNameSuffix;
            Password = password;
            NotificationEmails = notificationEmails;
            AllowSelfEnrolment = allowSelfEnrolment;
            HideInLearningPortal = hideInLearningPortal;
            DiagnosticObjectiveSelection = diagnosticObjectiveSelection;
            NumberOfLearning = numberOfLearning;
            NumberOfDiagnostic = numberOfDiagnostic;
        }
        public string CustomisationName { get; set; }
        public string CustomisationNameSuffix { get; set; }
        public string? Password { get; set; }
        public string? NotificationEmails { get; set; }
        public bool AllowSelfEnrolment { get; set; }
        public bool HideInLearningPortal { get; set; }
        public bool DiagnosticObjectiveSelection { get; set; }
        public bool NumberOfLearning { get; set; }
        public bool NumberOfDiagnostic { get; set; }
    }
}

