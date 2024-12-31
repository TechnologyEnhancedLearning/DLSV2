using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Import
{
    public class AddAssessmentQuestionsViewModel(
        DetailFramework framework,
        int newCompetencies,
        int existingCompetencies,
        IEnumerable<AssessmentQuestion> defaultQuestions,
        SelectList questionSelectList
        ) : AddAssessmentQuestionsFormData
    {
        public int FrameworkID { get; set; } = framework.ID;
        public string FrameworkName { get; set; } = framework.FrameworkName;
        public string FrameworkVocabularySingular { get; set; } = FrameworkVocabularyHelper.VocabularySingular(framework.FrameworkConfig);
        public string FrameworkVocabularyPlural { get; set; } = FrameworkVocabularyHelper.VocabularyPlural(framework.FrameworkConfig);
        public int PublishStatusID { get; set; } = framework.PublishStatusID;
        public int NewCompetencies { get; set; } = newCompetencies;
        public int ExistingCompetencies { get; set; } = existingCompetencies;
        public IEnumerable<AssessmentQuestion>? DefaultQuestions { get; set; } = defaultQuestions;
        public SelectList? QuestionSelectList { get; set; } = questionSelectList;
    }
}
