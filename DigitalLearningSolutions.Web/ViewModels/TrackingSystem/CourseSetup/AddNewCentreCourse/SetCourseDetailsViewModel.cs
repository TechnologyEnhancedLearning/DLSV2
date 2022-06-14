namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;

    public class SetCourseDetailsViewModel : EditCourseDetailsFormData
    {
        public SetCourseDetailsViewModel() { }

        public SetCourseDetailsViewModel(CourseDetailsData data)
        {
            ApplicationId = data.ApplicationId;
            ApplicationName = data.ApplicationName;
            CustomisationName = data.CustomisationName;
            PasswordProtected = data.PasswordProtected;
            Password = data.Password;
            ReceiveNotificationEmails = data.ReceiveNotificationEmails;
            NotificationEmails = data.NotificationEmails;
            PostLearningAssessment = data.PostLearningAssessment;
            IsAssessed = data.IsAssessed;
            DiagAssess = data.DiagAssess;
            TutCompletionThreshold = data.TutCompletionThreshold;
            DiagCompletionThreshold = data.DiagCompletionThreshold;
        }

        public SetCourseDetailsViewModel(ApplicationDetails application)
        {
            ApplicationId = application.ApplicationId;
            ApplicationName = application.ApplicationName;
            PostLearningAssessment = application.PLAssess;
            DiagAssess = application.DiagAssess;
            IsAssessed = true;
        }
    }
}
