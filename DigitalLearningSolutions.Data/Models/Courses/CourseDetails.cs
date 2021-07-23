namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public class CourseDetails
    {
        public int CustomisationId { get; set; }
        public int CentreId { get; set; }
        public string ApplicationName { get; set; }
        public string CustomisationName { get; set; }
        public int CurrentVersion { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? LastAccessed { get; set; }
        public string? Password { get; set; }
        public string? NotificationEmails { get; set; }
        public bool PlAssess { get; set; }
        public bool IsAssessed { get; set; }
        public int TutCompletionThreshold { get; set; }
        public bool DiagAssess { get; set; }
        public int DiagCompletionThreshold { get; set; }
        public bool SelfRegister { get; set; }
        public bool DiagObjSelect { get; set; }
        public bool HideInLearnerPortal { get; set; }
        public bool Active { get; set; }
        public int DelegateCount { get; set; }
        public int CompletedCount { get; set; }
        public int CompleteWithinMonths { get; set; }
        public int ValidityMonths { get; set; }
        public bool Mandatory { get; set; }
        public bool AutoRefresh { get; set; }
        public int RefreshToCustomisationId { get; set; }
        public string? RefreshToApplicationName { get; set; }
        public string? RefreshToCustomisationName { get; set; }
        public int AutoRefreshMonths { get; set; }
        public bool ApplyLpDefaultsToSelfEnrol { get; set; }

        public int InProgressCount => DelegateCount - CompletedCount;

        public string CourseName => string.IsNullOrWhiteSpace(CustomisationName)
            ? ApplicationName
            : ApplicationName + " - " + CustomisationName;

        public string? RefreshToCourseName
        {
            get
            {
                if (RefreshToCustomisationId == 0)
                {
                    return "Same course";
                }

                if (string.IsNullOrWhiteSpace(RefreshToApplicationName))
                {
                    return null;
                }

                return string.IsNullOrWhiteSpace(RefreshToCustomisationName)
                    ? RefreshToApplicationName
                    : RefreshToApplicationName + " - " + RefreshToCustomisationName;
            }
        }
    }
}
