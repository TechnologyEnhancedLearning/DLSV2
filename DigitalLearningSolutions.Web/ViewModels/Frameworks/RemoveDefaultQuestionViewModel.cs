namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    public class RemoveDefaultQuestionViewModel
    {
        public BaseFramework BaseFramework { get; set; }
        public int AssessmentQuestionId { get; set; }
        public FrameworkDefaultQuestionUsage? FrameworkDefaultQuestionUsage { get; set; }
    }
}
