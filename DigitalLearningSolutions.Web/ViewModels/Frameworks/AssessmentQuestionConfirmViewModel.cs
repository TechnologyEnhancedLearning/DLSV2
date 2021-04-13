namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;

    public class AssessmentQuestionConfirmViewModel
    {
        public int FrameworkId { get; set; }
        public int FrameworkCompetencyId { get; set; }
        public string? Name { get; set; }
        public int AssessmentQuestionInputTypeID { get; set; }
        public Data.Models.SelfAssessments.AssessmentQuestion AssessmentQuestion { get; set; }
        public string? FrameworkConfig { get; set; }
        public string VocabSingular()
        {
            return FrameworkVocabularyHelper.VocabularySingular(FrameworkConfig);
        }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(FrameworkConfig);
        }
    }
}
