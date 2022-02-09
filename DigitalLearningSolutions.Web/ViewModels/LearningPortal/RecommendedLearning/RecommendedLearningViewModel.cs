namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class RecommendedLearningViewModel : BaseSearchablePageViewModel
    {
        public RecommendedLearningViewModel(
            SelfAssessment selfAssessment,
            IEnumerable<RecommendedResource> recommendedResources,
            bool apiIsAccessible,
            string? searchString,
            int page
        ) : base(
            searchString,
            page,
            false,
            nameof(RecommendedResource.RecommendationScore),
            GenericSortingHelper.Descending,
            itemsPerPage: DefaultItemsPerPage,
            searchLabel: "Search resources"
        )
        {
            ApiIsAccessible = apiIsAccessible;
            SelfAssessment = selfAssessment;

            var searchedItems = GenericSearchHelper.SearchItems(recommendedResources, SearchString);

            var sortedResources = GenericSortingHelper.SortAllItems(
                searchedItems.AsQueryable(),
                nameof(RecommendedResource.RecommendationScore),
                GenericSortingHelper.Descending
            ).ToList();

            MatchingSearchResults = sortedResources.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(sortedResources);

            RecommendedResources =
                paginatedItems.Select(r => new SearchableRecommendedResourceViewModel(r, selfAssessment.Id));
        }

        public SelfAssessment SelfAssessment { get; set; }

        public IEnumerable<SearchableRecommendedResourceViewModel> RecommendedResources { get; set; }

        public bool ApiIsAccessible { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();

        public override bool NoDataFound => !RecommendedResources.Any() && NoSearchOrFilter;
    }
}
