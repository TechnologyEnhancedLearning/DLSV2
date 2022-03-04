namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class MyFrameworksViewModel : BaseSearchablePageViewModel<BrandedFramework>
    {
        public readonly IEnumerable<BrandedFramework> BrandedFrameworks;
        public readonly bool IsFrameworkDeveloper;

        public MyFrameworksViewModel(
            SearchSortFilterPaginateResult<BrandedFramework> result,
            bool isFrameworkDeveloper
        ) : base(result, false)
        {
            BrandedFrameworks = result.ItemsToDisplay;
            IsFrameworkDeveloper = isFrameworkDeveloper;
        }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            FrameworkSortByOptions.FrameworkName,
            FrameworkSortByOptions.FrameworkOwner,
            FrameworkSortByOptions.FrameworkCreatedDate,
            FrameworkSortByOptions.FrameworkPublishStatus
        };

        public override bool NoDataFound => !BrandedFrameworks.Any() && NoSearchOrFilter;
    }
}
