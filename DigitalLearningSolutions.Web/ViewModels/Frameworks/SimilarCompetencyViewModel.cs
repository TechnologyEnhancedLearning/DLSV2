namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using System.Collections.Generic;
    public class SimilarCompetencyViewModel
    {
        public int MatchingSearchResults { get; set; }
        public FrameworkCompetency Competency { get; set; }
        public int FrameworkId { get; set; }
        public int? FrameworkGroupId { get; set; }
        public int FrameworkCompetencyId { get; set; }
        public IEnumerable<FrameworkCompetency> SameCompetency { get; set; }
    }
}
