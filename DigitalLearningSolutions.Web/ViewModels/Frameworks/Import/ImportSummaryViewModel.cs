using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Import
{
    public class ImportSummaryViewModel
    {
        public ImportSummaryViewModel(BulkCompetenciesData data)
        {
            FrameworkName = data.FrameworkName;
            PublishStatusID = data.PublishStatusID;
            FrameworkVocabularySingular = FrameworkVocabularyHelper.VocabularySingular(data.FrameworkVocubulary);
            FrameworkVocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(data.FrameworkVocubulary);
            ToProcessCount = data.CompetenciesToProcessCount;
            CompetenciesToAddCount = data.CompetenciesToAddCount;
            CompetenciesToReorderCount = data.CompetenciesToReorderCount;
            ToUpdateOrSkipCount = data.CompetenciesToUpdateCount;
            AddAssessmentQuestionsOption = data.AddAssessmentQuestionsOption;
            AddDefaultAssessmentQuestions = data.AddDefaultAssessmentQuestions;
            AddCustomAssessmentQuestion = data.AddCustomAssessmentQuestion;
            DefaultAssessmentQuestionIDs = data.DefaultQuestionIDs;
            CustomAssessmentQuestionID = data.CustomAssessmentQuestionID;
            ReorderCompetenciesOption = data.ReorderCompetenciesOption;
        }
        public string? FrameworkName { get; set; }
        public int PublishStatusID { get; set; }
        public string FrameworkVocabularySingular { get; set; }
        public string FrameworkVocabularyPlural { get; set; }
        public int ToProcessCount { get; set; }
        public int CompetenciesToAddCount { get; set; }
        public int CompetenciesToReorderCount { get; set; }
        public int ToUpdateOrSkipCount { get; set; }
        public int AddAssessmentQuestionsOption { get; set; } = 1; //1 = only added, 2 = added and updated, 3 = all uploaded
        public bool AddDefaultAssessmentQuestions { get; set; }
        public bool AddCustomAssessmentQuestion { get; set; }
        public List<int> DefaultAssessmentQuestionIDs { get; set; }
        public int? CustomAssessmentQuestionID { get; set; }
        public int ReorderCompetenciesOption { get; set; } = 1; //1 = ignore order, 2 = apply order
    }
}
