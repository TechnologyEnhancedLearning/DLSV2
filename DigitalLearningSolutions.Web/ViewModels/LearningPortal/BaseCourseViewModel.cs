namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public abstract class BaseCourseViewModel : NamedItemViewModel
    {
        public sealed override string Name { get; set; }
        public int Id { get; }
        public bool HasDiagnosticAssessment { get; }
        public bool HasLearningContent { get; }
        public bool HasLearningAssessmentAndCertification { get; }
        public string LaunchUrl { get; }

        protected BaseCourseViewModel(BaseCourse course, IConfiguration config)
        {
            Name = course.CourseName;
            Id = course.CustomisationID;
            HasDiagnosticAssessment = course.HasDiagnostic;
            HasLearningContent = course.HasLearning;
            HasLearningAssessmentAndCertification = course.IsAssessed;
            LaunchUrl = config.GetLaunchUrl(course.CustomisationID);
        }
    }
}
