﻿namespace DigitalLearningSolutions.Web.ViewModels.RoleProfiles
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;
    public class AllRoleProfilesViewModel : BaseRoleProfilesPageViewModel
    {
        public readonly IEnumerable<RoleProfile> RoleProfiles;
        public readonly bool IsWorkforceManager;
        public override SelectList RoleProfileSortByOptions { get; } = new SelectList(new[]
    {
            RoleProfileSortByOptionTexts.RoleProfileName,
            RoleProfileSortByOptionTexts.RoleProfileOwner,
            RoleProfileSortByOptionTexts.RoleProfileCreatedDate,
            RoleProfileSortByOptionTexts.RoleProfilePublishStatus,
            RoleProfileSortByOptionTexts.RoleProfileBrand,
            RoleProfileSortByOptionTexts.RoleProfileNationalRoleGroup,
            RoleProfileSortByOptionTexts.RoleProfileNationalRoleProfile
        });
        public AllRoleProfilesViewModel(
            IEnumerable<RoleProfile> roleProfiles,
            string? searchString,
            string sortBy,
            string sortDirection,
            int page
        ) : base(searchString, sortBy, sortDirection, page)
        {
            var sortedItems = SortingHelper.SortRoleProfileItems(
                roleProfiles,
                sortBy,
                sortDirection
            );
            var filteredItems = SearchHelper.FilterRoleProfiles(sortedItems, SearchString, 60, false).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = PaginateItems(filteredItems);
            RoleProfiles = paginatedItems.Cast<RoleProfile>();
        }
    }
}
