namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Import
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;

    public class ImportCompetenciesViewModel : ImportCompetenciesFormData
    {
        public ImportCompetenciesViewModel(DetailFramework framework, bool isNotBlank)
        {
            FrameworkID = framework.ID;
            FrameworkName = framework.FrameworkName;
            FrameworkVocabularySingular = FrameworkVocabularyHelper.VocabularySingular(framework.FrameworkConfig);
            FrameworkVocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(framework.FrameworkConfig);
            PublishStatusID = framework.PublishStatusID;
            IsNotBlank = isNotBlank;
        }
        public int FrameworkID { get; set; }
        public string FrameworkName { get; set; }
        public string FrameworkVocabularySingular { get; set; }
        public string FrameworkVocabularyPlural { get; set; }
        public int PublishStatusID { get; set; }
        public bool IsNotBlank { get; set; }

    }
}
