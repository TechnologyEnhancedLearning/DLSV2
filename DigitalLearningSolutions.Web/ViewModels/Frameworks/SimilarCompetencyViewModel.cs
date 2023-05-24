namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    public class SimilarCompetencyViewModel
    {
        public int MatchingSearchResults { get; set; }
        public FrameworkCompetency Competency { get; set; }
        public int FrameworkId { get; set; }
        public int? FrameworkGroupId { get; set; }
        public int FrameworkCompetencyId { get; set; }
        public string FrameworkConfig { get; set; }
        public IEnumerable<FrameworkCompetency> SameCompetency { get; set; }
        public string selectedFlagIds { get; set; }

        public string VocabSingular()
        {
            return FrameworkVocabularyHelper.VocabularySingular(FrameworkConfig);
        }
    }
}
