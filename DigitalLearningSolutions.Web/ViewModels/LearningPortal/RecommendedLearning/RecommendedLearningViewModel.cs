namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public class RecommendedLearningViewModel
    {
        public RecommendedLearningViewModel(
            SelfAssessment selfAssessment,
            IEnumerable<RecommendedResource> recommendedResources
        )
        {
            SelfAssessment = selfAssessment;

            // TODO HEEDLS-650 Search/Pagination
            var resourcesToDisplay = recommendedResources.Take(10);

            RecommendedResources = resourcesToDisplay.Select(r => new SearchableRecommendedResourceViewModel(r));
        }

        public SelfAssessment SelfAssessment { get; set; }

        public IEnumerable<SearchableRecommendedResourceViewModel> RecommendedResources { get; set; }
    }
}
