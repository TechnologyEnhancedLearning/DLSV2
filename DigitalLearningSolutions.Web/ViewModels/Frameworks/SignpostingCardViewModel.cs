namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class SignpostingCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AssessmentQuestion { get; set; }
        public int MinimumResultMatch { get; set; }
        public int MaximumResultMatch { get; set; }
        public string CompareResultTo { get; set; }
        public int AssessmentQuestionParameterId { get; set; }
    }
}
