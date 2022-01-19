namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class RecommendedLearningViewModel
    {
        public RecommendedLearningViewModel(
            SelfAssessment selfAssessment,
            IEnumerable<RecommendedResource> recommendedResources,
            bool resourcesSourcedFromFallbackData
        )
        {
            ResourcesSourcedFromFallbackData = resourcesSourcedFromFallbackData;
            SelfAssessment = selfAssessment;

            var sortedResources = GenericSortingHelper.SortAllItems(
                recommendedResources.AsQueryable(),
                nameof(RecommendedResource.RecommendationScore),
                BaseSearchablePageViewModel.Descending
            );

            // TODO HEEDLS-650 Search/Pagination
            var resourcesToDisplay = sortedResources.Take(10);

            RecommendedResources =
                resourcesToDisplay.Select(r => new SearchableRecommendedResourceViewModel(r, selfAssessment.Id));
        }

        public SelfAssessment SelfAssessment { get; set; }

        public IEnumerable<SearchableRecommendedResourceViewModel> RecommendedResources { get; set; }

        public bool ResourcesSourcedFromFallbackData { get; set; }
    }
}
