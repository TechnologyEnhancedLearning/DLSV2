namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc.Rendering;
    public class MyFrameworksViewModel : BaseSearchablePageViewModel
    {
        public readonly IEnumerable<BrandedFramework> BrandedFrameworks;
        public readonly bool IsFrameworkDeveloper;
        
        public override List<SelectListItem> SortByOptions { get; } = new List<SelectListItem>
        {
            item1,  item2, item3, item4
        };

        private static SelectListItem item1 = new SelectListItem(FrameworkSortByOptionTexts.FrameworkName, nameof(BaseFramework.FrameworkName));
        private static SelectListItem item2 = new SelectListItem(FrameworkSortByOptionTexts.FrameworkOwner, nameof(BaseFramework.Owner));
        private static SelectListItem item3 = new SelectListItem(FrameworkSortByOptionTexts.FrameworkCreatedDate, nameof(BaseFramework.CreatedDate));
        private static SelectListItem item4 = new SelectListItem(FrameworkSortByOptionTexts.FrameworkPublishStatus, nameof(BaseFramework.PublishStatus));

        public MyFrameworksViewModel(
            IEnumerable<BrandedFramework> brandedFrameworks,
            string? searchString,
            string sortBy,
            string sortDirection,
            int page,
            bool isFrameworkDeveloper
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
            IsFrameworkDeveloper = isFrameworkDeveloper;
        }
    }
}
