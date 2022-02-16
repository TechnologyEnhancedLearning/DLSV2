namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using DigitalLearningSolutions.Web.ViewModels.RoleProfiles;

    /// <summary>
    /// This is the older version of the SortingHelper. For future search/sort implementations we should
    /// be using the generic version <see cref="GenericSortingHelper"/> and implementing BaseSearchableItem
    /// on the entity to be sorted
    /// </summary>
    public static class SortingHelper
    {
        public static IEnumerable<RoleProfile> SortRoleProfileItems(
            IEnumerable<RoleProfile> roleProfiles,
            string sortBy,
            string sortDirection
        )
        {
            return sortBy switch
            {
                RoleProfileSortByOptionTexts.RoleProfileName => sortDirection == BaseRoleProfilesPageViewModel.DescendingText
                    ? roleProfiles.OrderByDescending(roleProfile => roleProfile.RoleProfileName)
                    : roleProfiles.OrderBy(roleProfile => roleProfile.RoleProfileName),
                RoleProfileSortByOptionTexts.RoleProfileOwner => sortDirection == BaseRoleProfilesPageViewModel.DescendingText
                    ? roleProfiles.OrderByDescending(roleProfile => roleProfile.Owner)
                    : roleProfiles.OrderBy(roleProfile => roleProfile.Owner),
                RoleProfileSortByOptionTexts.RoleProfileCreatedDate => sortDirection == BaseRoleProfilesPageViewModel.DescendingText
                    ? roleProfiles.OrderByDescending(roleProfile => roleProfile.CreatedDate)
                    : roleProfiles.OrderBy(roleProfile => roleProfile.CreatedDate),
                RoleProfileSortByOptionTexts.RoleProfilePublishStatus => sortDirection == BaseRoleProfilesPageViewModel.DescendingText
                    ? roleProfiles.OrderByDescending(roleProfile => roleProfile.PublishStatusID)
                    : roleProfiles.OrderBy(roleProfile => roleProfile.PublishStatusID),
                RoleProfileSortByOptionTexts.RoleProfileBrand => sortDirection == BaseRoleProfilesPageViewModel.DescendingText
                    ? roleProfiles.OrderByDescending(roleProfile => roleProfile.Brand)
                    : roleProfiles.OrderBy(roleProfile => roleProfile.Brand),
                RoleProfileSortByOptionTexts.RoleProfileNationalRoleProfile => sortDirection == BaseRoleProfilesPageViewModel.DescendingText
                ? roleProfiles.OrderByDescending(roleProfile => roleProfile.NRPRole)
                : roleProfiles.OrderBy(roleProfile => roleProfile.NRPRole),
                RoleProfileSortByOptionTexts.RoleProfileNationalRoleGroup => sortDirection == BaseRoleProfilesPageViewModel.DescendingText
                ? roleProfiles.OrderByDescending(roleProfile => roleProfile.NRPProfessionalGroup)
                : roleProfiles.OrderBy(roleProfile => roleProfile.NRPProfessionalGroup),
                _ => roleProfiles
            };
        }
    }
}
