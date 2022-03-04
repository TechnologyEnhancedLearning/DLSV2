namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllFrameworksViewModel : BaseSearchablePageViewModel<BrandedFramework>
    {
        public readonly IEnumerable<BrandedFramework> BrandedFrameworks;

        public AllFrameworksViewModel(
            SearchSortFilterPaginateResult<BrandedFramework> result
        ) : base(result, false)
        {
            BrandedFrameworks = result.ItemsToDisplay;
        }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            FrameworkSortByOptions.FrameworkName,
            FrameworkSortByOptions.FrameworkOwner,
            FrameworkSortByOptions.FrameworkCreatedDate,
            FrameworkSortByOptions.FrameworkPublishStatus,
            FrameworkSortByOptions.FrameworkBrand,
            FrameworkSortByOptions.FrameworkCategory,
            FrameworkSortByOptions.FrameworkTopic,
        };

        public override bool NoDataFound => !BrandedFrameworks.Any() && NoSearchOrFilter;
    }
}
