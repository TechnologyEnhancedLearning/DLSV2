namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IBasePaginatedViewModel
    {
        public int MatchingSearchResults { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }

        public int ItemsPerPage { get; set; }

        IEnumerable<SelectListItem> ItemsPerPageSelectListItems { get; }
    }


    public abstract class BasePaginatedViewModel<T> : IBasePaginatedViewModel where T : BaseSearchableItem
    {
        protected BasePaginatedViewModel(PaginationResult<T> paginationResult)
        {
            Page = paginationResult.Page;
            ItemsPerPage = paginationResult.ItemsPerPage;
            MatchingSearchResults = paginationResult.MatchingSearchResults;
            TotalPages = paginationResult.TotalPages;
            JavascriptSearchSortFilterPaginateEnabled = paginationResult.TotalItems <= 100;
        }

        public int MatchingSearchResults { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }

        public int ItemsPerPage { get; set; }

        public bool JavascriptSearchSortFilterPaginateEnabled { get; set; }

        public IEnumerable<SelectListItem> ItemsPerPageSelectListItems =>
            SelectListHelper.MapOptionsToSelectListItems(ItemsPerPageOptions);

        public virtual IEnumerable<(int, string)> ItemsPerPageOptions { get; } = new[]
        {
            (10, "10"),
            (25, "25"),
            (50, "50"),
            (100, "100"),
        };
    }
}
