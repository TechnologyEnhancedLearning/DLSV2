using DigitalLearningSolutions.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Import
{
    public class AddQuestionsToWhichCompetenciesViewModel
    {
        public AddQuestionsToWhichCompetenciesViewModel
            (
            int frameworkId,
            string frameworkName,
            string frameworkVocabulary,
            List<int> defaultQuestions,
            int? customQuestionId,
            int addAssessmentQuestionsOption,
            int competenciesToProcessCount,
            int competenciesToAddCount,
            int competenciesToUpdateCount)
        {
            FrameworkID = frameworkId;
            FrameworkName = frameworkName;
            FrameworkVocabularySingular = FrameworkVocabularyHelper.VocabularySingular(frameworkVocabulary);
            FrameworkVocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(frameworkVocabulary);
            TotalQuestions = defaultQuestions.Count + (customQuestionId != null ? 1 : 0);
            AddAssessmentQuestionsOption = addAssessmentQuestionsOption;
            CompetenciesToProcessCount = competenciesToProcessCount;
            CompetenciesToAddCount = competenciesToAddCount;
            CompetenciesToUpdateCount = competenciesToUpdateCount;
        }
        public int FrameworkID { get; set; }
        public string FrameworkName { get; set; }
        public string FrameworkVocabularySingular { get; set; }
        public string FrameworkVocabularyPlural { get; set; }
        public int TotalQuestions { get; set; }
        public int AddAssessmentQuestionsOption { get; set; } //1 = only added, 2 = added and updated, 3 = all uploaded
        public int CompetenciesToProcessCount { get; set; }
        public int CompetenciesToAddCount { get; set; }
        public int CompetenciesToUpdateCount { get; set; }
        
    }
}
