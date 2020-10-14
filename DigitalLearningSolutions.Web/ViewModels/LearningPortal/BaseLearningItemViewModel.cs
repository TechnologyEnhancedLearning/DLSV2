namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public abstract class BaseLearningItemViewModel
    {
        public string Name { get; set; }
        public int Id { get; }
        public bool HasDiagnosticAssessment { get; }
        public bool HasLearningContent { get; }
        public bool HasLearningAssessmentAndCertification { get; }
        public bool IsSelfAssessment { get; }
        public bool UseFilteredApi { get;  }

        protected BaseLearningItemViewModel(BaseLearningItem course)
        {
            Name = course.Name;
            Id = course.Id;
            HasDiagnosticAssessment = course.HasDiagnostic;
            HasLearningContent = course.HasLearning;
            HasLearningAssessmentAndCertification = course.IsAssessed;
            IsSelfAssessment = course.IsSelfAssessment;
            UseFilteredApi = course.UseFilteredApi;
        }
    }
}
