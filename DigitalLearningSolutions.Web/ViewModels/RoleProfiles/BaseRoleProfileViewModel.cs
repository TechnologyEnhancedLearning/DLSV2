namespace DigitalLearningSolutions.Web.ViewModels.RoleProfiles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public abstract class BaseRoleProfilesPageViewModel
    {
        [BindProperty] public string SortDirection { get; set; }
        [BindProperty] public string SortBy { get; set; }
        public int Page { get; protected set; }
        public int TotalPages { get; protected set; }
        public int MatchingSearchResults;
        public abstract SelectList RoleProfileSortByOptions { get; }

        public const string DescendingText = "Descending";
        public const string AscendingText = "Ascending";

        private const int ItemsPerPage = 12;

        public readonly string? SearchString;

        protected BaseRoleProfilesPageViewModel(
            string? searchString,
            string sortBy,
            string sortDirection,
            int page
        )
        {
            SortBy = sortBy;
            SortDirection = sortDirection;
            SearchString = searchString;
            Page = page;
        }
        protected IEnumerable<RoleProfile> PaginateItems(IList<RoleProfile> items)
        {
            if (items.Count > ItemsPerPage)
            {
                items = items.Skip(OffsetFromPageNumber(Page)).Take(ItemsPerPage).ToList();
            }

            return items;
        }
        protected void SetTotalPages()
        {
            TotalPages = (int)Math.Ceiling(MatchingSearchResults / (double)ItemsPerPage);
            if (Page < 1 || Page > TotalPages)
            {
                Page = 1;
            }
        }

        private int OffsetFromPageNumber(int pageNumber) =>
            (pageNumber - 1) * ItemsPerPage;
    }

    public static class RoleProfileSortByOptionTexts
    {
        public const string
            RoleProfileName = "Profile Name",
            RoleProfileOwner = "Owner",
            RoleProfileCreatedDate = "Created Date",
            RoleProfilePublishStatus = "Publish Status",
            RoleProfileBrand = "Brand",
            RoleProfileNationalRoleGroup = "National Job Group",
            RoleProfileNationalRoleProfile = "National Job Profile";
    }
}
