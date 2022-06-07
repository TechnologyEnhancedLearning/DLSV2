namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class RecommendedLearningViewModel : BaseSearchablePageViewModel<RecommendedResource>
    {
        public RecommendedLearningViewModel(
            CurrentSelfAssessment selfAssessment,
            SearchSortFilterPaginationResult<RecommendedResource> result,
            bool apiIsAccessible
        ) : base(
            result,
            false,
            searchLabel: "Search resources"
        )
        {
            ApiIsAccessible = apiIsAccessible;
            SelfAssessment = selfAssessment;

            RecommendedResources =
                result.ItemsToDisplay.Select(
                    r => new SearchableRecommendedResourceViewModel(
                        r,
                        selfAssessment.Id,
                        result.GetReturnPageQuery($"{r.LearningHubReferenceId}-card")
                    )
                );
        }

        public CurrentSelfAssessment SelfAssessment { get; set; }

        public IEnumerable<SearchableRecommendedResourceViewModel> RecommendedResources { get; set; }

        public bool ApiIsAccessible { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();

        public override bool NoDataFound => !RecommendedResources.Any() && NoSearchOrFilter;

        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(SelfAssessment.Vocabulary);
        }
    }
}
