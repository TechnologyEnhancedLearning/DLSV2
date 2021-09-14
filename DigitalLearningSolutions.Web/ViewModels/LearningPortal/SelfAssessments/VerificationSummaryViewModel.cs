namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Web.Helpers;
    public class VerificationSummaryViewModel
    {
        public int SelfAssessmentId { get; set; }
        public string Supervisor { get; set; }
        public string? Vocubulary { get; set; }
        public string? SelfAssessmentName { get; set; }
        public int ResultCount { get; set; }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(Vocubulary);
        }
    }
}
