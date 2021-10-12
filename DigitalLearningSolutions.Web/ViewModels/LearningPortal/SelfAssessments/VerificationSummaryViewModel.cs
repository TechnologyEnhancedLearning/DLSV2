namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    public class VerificationSummaryViewModel
    {
        public string Supervisor { get; set; }
        public CurrentSelfAssessment? SelfAssessment { get; set; }
        public int ResultCount { get; set; }
        public string VocabPlural()
        {
            if (SelfAssessment != null)
            {
                return FrameworkVocabularyHelper.VocabularyPlural(SelfAssessment.Vocabulary);
            }
            else
            {
                return "Capabilities";
            }
        }
    }
}
