using DigitalLearningSolutions.Web.Helpers;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Import
{
    public class ApplyCompetencyOrderingViewModel
    {
        public ApplyCompetencyOrderingViewModel
            (
            int frameworkId,
            string frameworkName,
            string frameworkVocabulary,
            int competenciesToReorderCount,
            int reorderCompetenciesOption
            )
        {
            FrameworkID = frameworkId;
            FrameworkName = frameworkName;
            FrameworkVocabularySingular = FrameworkVocabularyHelper.VocabularySingular(frameworkVocabulary);
            FrameworkVocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(frameworkVocabulary);
            CompetenciesToReorderCount = competenciesToReorderCount;
            ReorderCompetenciesOption = reorderCompetenciesOption;
        }
        public int FrameworkID { get; set; }
        public string FrameworkName { get; set; }
        public string FrameworkVocabularySingular { get; set; }
        public string FrameworkVocabularyPlural { get; set; }
        public int ReorderCompetenciesOption { get; set; } = 1; //1 = ignore order, 2 = apply order
        public int CompetenciesToReorderCount { get; set; }
    }
}
