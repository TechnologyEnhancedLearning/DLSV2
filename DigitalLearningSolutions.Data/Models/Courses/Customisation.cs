namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class Customisation
    {
        public Customisation(
            int centreId,
            int applicationId,
            string customisationName,
            string? password,
            bool selfRegister,
            int tutCompletionThreshold,
            bool isAssessed,
            int diagCompletionThreshold,
            bool diagObjSelect,
            bool hideInLearnerPortal,
            string? notificationEmails
        )
        {
            CentreId = centreId;
            ApplicationId = applicationId;
            CustomisationName = customisationName;
            Password = password;
            SelfRegister = selfRegister;
            TutCompletionThreshold = tutCompletionThreshold;
            IsAssessed = isAssessed;
            DiagCompletionThreshold = diagCompletionThreshold;
            DiagObjSelect = diagObjSelect;
            HideInLearnerPortal = hideInLearnerPortal;
            NotificationEmails = notificationEmails;
        }

        public int CentreId { get; set; }
        public int ApplicationId { get; set; }
        public string? CustomisationName { get; set; }
        public string? Password { get; set; }
        public bool SelfRegister { get; set; }
        public int TutCompletionThreshold { get; set; }
        public bool IsAssessed { get; set; }
        public int DiagCompletionThreshold { get; set; }
        public bool DiagObjSelect { get; set; }
        public bool HideInLearnerPortal { get; set; }
        public string? NotificationEmails { get; set; }
    }
}
