using DigitalLearningSolutions.Web.Helpers;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class SetMinimumOptionalCompetenciesFormData
    {
        [MaxOptionalCount(nameof(OptionalCompetenciesCount))]
        public int? MinimumOptionalCompetencies { get; set; }
        public int OptionalCompetenciesCount { get; set; }
        public int ID { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
    }
}
