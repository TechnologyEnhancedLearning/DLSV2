namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;
    public class CompetencyAssessmentQuestionsViewModel
    {
        public int FrameworkId { get; set; }
        public int FrameworkCompetencyId { get; set; }
        public string? CompetencyName { get; set; }
        public SelectList? QuestionSelectList { get; set; }
        public IEnumerable<AssessmentQuestion>? AssessmentQuestions { get; set; }
        public int assessmentQuestionId { get; set; }
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
