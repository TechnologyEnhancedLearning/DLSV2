namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using DigitalLearningSolutions.Data.Models;

    public abstract class BaseLearningItemViewModel
    {
        protected BaseLearningItemViewModel(BaseLearningItem course)
        {
            Name = course.Name;
            Id = course.Id;
            HasDiagnosticAssessment = course.HasDiagnostic;
            HasLearningContent = course.HasLearning;
            HasLearningAssessmentAndCertification = course.IsAssessed;
            IsSelfAssessment = course.IsSelfAssessment;
            SelfRegister = course.SelfRegister;
            IncludesSignposting = course.IncludesSignposting;
        }

        public string Name { get; set; }
        public int Id { get; }
        public bool HasDiagnosticAssessment { get; }
        public bool HasLearningContent { get; }
        public bool HasLearningAssessmentAndCertification { get; }
        public bool IsSelfAssessment { get; }
        public bool SelfRegister { get; }
        public bool IncludesSignposting { get; }
    }
}
