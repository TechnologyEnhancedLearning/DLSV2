namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    public class FrameworkCompetencyViewModel
    {
        public int FrameworkId { get; set; }
        public int? FrameworkCompetencyGroupId { get; set; }
        public FrameworkCompetency FrameworkCompetency { get; set; }
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
