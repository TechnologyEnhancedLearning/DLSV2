namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;
    public class AllFrameworksViewModel : BaseFrameworksPageViewModel
    {
        public readonly IEnumerable<BrandedFramework> BrandedFrameworks;
        public override SelectList FrameworkSortByOptions { get; } = new SelectList(new[]
    {
            FrameworkSortByOptionTexts.FrameworkName,
            FrameworkSortByOptionTexts.FrameworkOwner,
            FrameworkSortByOptionTexts.FrameworkCreatedDate,
            FrameworkSortByOptionTexts.FrameworkPublishStatus,
            FrameworkSortByOptionTexts.FrameworkBrand,
            FrameworkSortByOptionTexts.FrameworkCategory,
            FrameworkSortByOptionTexts.FrameworkTopic
        });
        public AllFrameworksViewModel(
            IEnumerable<BrandedFramework> brandedFrameworks,
            string? searchString,
            string sortBy,
            string sortDirection,
            int page
        ) : base(searchString, sortBy, sortDirection, page)
        {
            var sortedItems = SortingHelper.SortFrameworkItems(
                brandedFrameworks,
                sortBy,
                sortDirection
            );
            var filteredItems = SearchHelper.FilterFrameworks(sortedItems, SearchString, 60, false).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = PaginateItems(filteredItems);
            BrandedFrameworks = paginatedItems.Cast<BrandedFramework>();
        }
    }
}
