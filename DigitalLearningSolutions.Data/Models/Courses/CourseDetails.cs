namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public class CourseDetails : Course
    {
        public int CurrentVersion { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastAccessed { get; set; }
        public string? Password { get; set; }
        public string? NotificationEmails { get; set; }
        public bool PostLearningAssessment { get; set; }
        public bool IsAssessed { get; set; }
        public int TutCompletionThreshold { get; set; }
        public bool DiagAssess { get; set; }
        public int DiagCompletionThreshold { get; set; }
        public bool SelfRegister { get; set; }
        public bool DiagObjSelect { get; set; }
        public bool HideInLearnerPortal { get; set; }
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
        public int CourseCategoryId { get; set; }

        public int InProgressCount => DelegateCount - CompletedCount;

        public string? RefreshToCourseName
        {
            get
            {
                if (RefreshToCustomisationId == 0 || RefreshToCustomisationId == CustomisationId)
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
