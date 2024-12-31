using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Import
{
    public class AddAssessmentQuestionsViewModel(
        int frameworkId,
        string frameworkName,
        string frameworkVocabulary,
        int publishStatusId,
        int newCompetencies,
        int existingCompetencies,
        IEnumerable<AssessmentQuestion> defaultQuestions,
        SelectList questionSelectList
        ) : AddAssessmentQuestionsFormData
    {
        public int FrameworkID { get; set; } = frameworkId;
        public string FrameworkName { get; set; } = frameworkName;
        public string FrameworkVocabularySingular { get; set; } = FrameworkVocabularyHelper.VocabularySingular(frameworkVocabulary);
        public string FrameworkVocabularyPlural { get; set; } = FrameworkVocabularyHelper.VocabularyPlural(frameworkVocabulary);
        public int PublishStatusID { get; set; } = publishStatusId;
        public int NewCompetencies { get; set; } = newCompetencies;
        public int ExistingCompetencies { get; set; } = existingCompetencies;
        public IEnumerable<AssessmentQuestion>? DefaultQuestions { get; set; } = defaultQuestions;
        public SelectList? QuestionSelectList { get; set; } = questionSelectList;
    }
}
