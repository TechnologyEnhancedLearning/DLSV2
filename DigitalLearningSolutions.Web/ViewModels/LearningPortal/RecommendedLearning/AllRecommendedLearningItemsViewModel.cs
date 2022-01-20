namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.LearningResources;

    public class AllRecommendedLearningItemsViewModel
    {
        public AllRecommendedLearningItemsViewModel(IEnumerable<RecommendedResource> resources, int selfAssessmentId)
        {
            RecommendedResources =
                resources.OrderByDescending(r => r.RecommendationScore).Select(r => new SearchableRecommendedResourceViewModel(r, selfAssessmentId));
        }

        public IEnumerable<SearchableRecommendedResourceViewModel> RecommendedResources { get; set; }
    }
}
