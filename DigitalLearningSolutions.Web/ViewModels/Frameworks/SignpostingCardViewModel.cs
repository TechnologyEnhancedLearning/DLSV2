namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class SignpostingCardViewModel
    {
        public int? AssessmentQuestionId { get; set; }
        public int? CompetencyLearningResourceId { get; set; }
        public int? ResourceRefId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Catalogue { get; set; }
        public string ResourceType { get; set; }
        public decimal Rating { get; set; }
        public string AssessmentQuestion { get; set; }
        public int MinimumResultMatch { get; set; }
        public int MaximumResultMatch { get; set; }
        public string CompareResultTo { get; set; }
        public bool Essential { get; set; }
        public bool ParameterHasNotBeenSet { get; set; }
        public bool UnmatchedResource { get; set; }
    }
}
