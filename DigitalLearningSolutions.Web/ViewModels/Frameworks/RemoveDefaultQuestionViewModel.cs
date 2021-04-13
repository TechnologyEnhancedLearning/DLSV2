namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    public class RemoveDefaultQuestionViewModel
    {
        public int FrameworkId { get; set; }
        public int AssessmentQuestionId { get; set; }
        public FrameworkDefaultQuestionUsage? FrameworkDefaultQuestionUsage { get; set; }
    }
}
