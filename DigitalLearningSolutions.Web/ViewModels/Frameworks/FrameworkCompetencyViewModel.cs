namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;

    public class FrameworkCompetencyViewModel
    {
        public DetailFramework DetailFramework { get; set; }
        public int? FrameworkCompetencyGroupId { get; set; }
        public FrameworkCompetency FrameworkCompetency { get; set; }
        public IEnumerable<CompetencyFlag> CompetencyFlags { get; set; }
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
