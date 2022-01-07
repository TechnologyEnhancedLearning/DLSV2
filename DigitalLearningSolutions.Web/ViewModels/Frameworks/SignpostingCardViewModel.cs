namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class SignpostingCardViewModel
    {
        public int? CompetencyLearningResourceId { get; set; }
        public string Name { get; set; }
        public string AssessmentQuestion { get; set; }
        public int MinimumResultMatch { get; set; }
        public int MaximumResultMatch { get; set; }
        public string CompareResultTo { get; set; }
        public bool Essential { get; set; }
    }
}
