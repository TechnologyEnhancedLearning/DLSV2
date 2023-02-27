namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using System.Collections.Generic;
    public class SimilarViewModel
    {
        public int MatchingSearchResults { get; set; }
        public string FrameworkName { get; set; }
        public IEnumerable<BrandedFramework> SimilarFrameworks { get; set; }
        public IEnumerable<BrandedFramework> SameFrameworks { get; set; }
    }
}
