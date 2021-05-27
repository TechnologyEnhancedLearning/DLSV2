namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;

    public class AssessmentQuestionConfirmViewModel
    {
        public DetailFramework DetailFramework { get; set; }
        public int FrameworkCompetencyId { get; set; }
        public string? Name { get; set; }
        public int AssessmentQuestionInputTypeID { get; set; }
        public Data.Models.SelfAssessments.AssessmentQuestion AssessmentQuestion { get; set; }
        public string VocabSingular()
        {
            return FrameworkVocabularyHelper.VocabularySingular(DetailFramework.FrameworkConfig);
        }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(DetailFramework.FrameworkConfig);
        }
    }
}
