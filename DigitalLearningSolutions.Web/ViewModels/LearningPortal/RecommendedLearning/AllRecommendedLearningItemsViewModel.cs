namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class AllRecommendedLearningItemsViewModel
    {
        public AllRecommendedLearningItemsViewModel(IEnumerable<RecommendedResource> resources, int selfAssessmentId)
        {
            RecommendedResources =
                resources.OrderByDescending(r => r.RecommendationScore).Select(
                    r => new SearchableRecommendedResourceViewModel(
                        r,
                        selfAssessmentId,
                        new ReturnPageQuery(1, $"{r.LearningHubReferenceId}-card")
                    )
                );
        }

        public IEnumerable<SearchableRecommendedResourceViewModel> RecommendedResources { get; set; }
    }
}
