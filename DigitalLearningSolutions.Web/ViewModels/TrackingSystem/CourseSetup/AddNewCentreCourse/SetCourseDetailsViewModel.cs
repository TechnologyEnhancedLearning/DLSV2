namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;

    public class SetCourseDetailsViewModel : EditCourseDetailsFormData
    {
        public SetCourseDetailsViewModel() { }

        public SetCourseDetailsViewModel(CourseDetailsTempData tempData)
        {
            ApplicationId = tempData.ApplicationId;
            ApplicationName = tempData.ApplicationName;
            CustomisationName = tempData.CustomisationName;
            PasswordProtected = tempData.PasswordProtected;
            Password = tempData.Password;
            ReceiveNotificationEmails = tempData.ReceiveNotificationEmails;
            NotificationEmails = tempData.NotificationEmails;
            PostLearningAssessment = tempData.PostLearningAssessment;
            IsAssessed = tempData.IsAssessed;
            DiagAssess = tempData.DiagAssess;
            TutCompletionThreshold = tempData.TutCompletionThreshold;
            DiagCompletionThreshold = tempData.DiagCompletionThreshold;
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
