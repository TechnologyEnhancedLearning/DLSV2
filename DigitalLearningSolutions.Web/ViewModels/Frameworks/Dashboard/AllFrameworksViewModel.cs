namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AllFrameworksViewModel : BaseSearchablePageViewModel
    {
        public readonly IEnumerable<BrandedFramework> BrandedFrameworks;
        
        public override List<SelectListItem> SortByOptions { get; } = new List<SelectListItem>
        {
            item1,  item2, item3, item4,item5, item6, item7
        };

        private static SelectListItem item1 = new SelectListItem(FrameworkSortByOptionTexts.FrameworkName, nameof(BaseFramework.FrameworkName));
        private static SelectListItem item2 = new SelectListItem(FrameworkSortByOptionTexts.FrameworkOwner, nameof(BaseFramework.Owner));
        private static SelectListItem item3 = new SelectListItem(FrameworkSortByOptionTexts.FrameworkCreatedDate, nameof(BaseFramework.CreatedDate));
        private static SelectListItem item4 = new SelectListItem(FrameworkSortByOptionTexts.FrameworkPublishStatus, nameof(BaseFramework.PublishStatus));
        private static SelectListItem item5 = new SelectListItem(FrameworkSortByOptionTexts.FrameworkBrand, nameof(BrandedFramework.Brand));
        private static SelectListItem item6 = new SelectListItem(FrameworkSortByOptionTexts.FrameworkCategory, nameof(BrandedFramework.Category));
        private static SelectListItem item7 = new SelectListItem(FrameworkSortByOptionTexts.FrameworkTopic, nameof(BrandedFramework.Topic));

        public AllFrameworksViewModel(
            IEnumerable<BrandedFramework> brandedFrameworks,
            string? searchString,
            string sortBy,
            string sortDirection,
            int page
        ) : base(searchString, sortBy, sortDirection, page)
        {
            var sortedItems = GenericSortingHelper.SortAllItems(
                brandedFrameworks.AsQueryable(),
                sortBy,
                sortDirection
            );
            var filteredItems = GenericSearchHelper.SearchItems(sortedItems, SearchString, 60, false).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = PaginateItems(filteredItems);
            BrandedFrameworks = paginatedItems.Cast<BrandedFramework>();
        }
    }
}
