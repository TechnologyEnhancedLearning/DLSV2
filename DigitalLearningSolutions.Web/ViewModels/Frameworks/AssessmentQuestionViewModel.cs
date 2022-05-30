namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AssessmentQuestionViewModel
    {
        public DetailFramework DetailFramework { get; set; }
        public int FrameworkCompetencyId { get; set; }
        public string? Name { get; set; }
        public AssessmentQuestionDetail AssessmentQuestionDetail { get; set; }
        public SelectList? InputTypeSelectList { get; set; }
        public string SupervisorName { get; set; }
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
