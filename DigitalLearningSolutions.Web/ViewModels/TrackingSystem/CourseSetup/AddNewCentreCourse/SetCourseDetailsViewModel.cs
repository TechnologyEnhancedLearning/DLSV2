namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;

    public class SetCourseDetailsViewModel : EditCourseDetailsFormData
    {
        public SetCourseDetailsViewModel() { }

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
