﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableAdminViewModel : BaseFilterableViewModel
    {
        public readonly bool CanShowDeactivateAdminButton;

        public SearchableAdminViewModel(
            AdminEntity admin,
            AdminAccount loggedInAdminAccount,
            ReturnPageQuery returnPageQuery
        )
        {
            Id = admin.AdminAccount.Id;
            Name = string.IsNullOrWhiteSpace(admin.UserAccount.FirstName) ? admin.UserAccount.LastName :
                    $"{admin.UserAccount.LastName}, {admin.UserAccount.FirstName}";
            CategoryName = admin.AdminAccount.CategoryName ?? "All";
            EmailAddress = admin.EmailForCentreNotifications;
            IsLocked = admin.UserAccount.FailedLoginCount >= AuthHelper.FailedLoginThreshold;
            IsActive = admin.AdminAccount.Active;
            CanShowDeactivateAdminButton =
                UserPermissionsHelper.LoggedInAdminCanDeactivateUser(admin.AdminAccount, loggedInAdminAccount);

            Tags = FilterableTagHelper.GetCurrentTagsForAdmin(admin);
            ReturnPageQuery = returnPageQuery;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string CategoryName { get; set; }

        public string CategoryFilter => nameof(AdminEntity.CategoryName) + FilteringHelper.Separator +
                                        nameof(AdminEntity.CategoryName) +
                                        FilteringHelper.Separator + CategoryName;

        public string? EmailAddress { get; set; }

        public bool IsLocked { get; set; }

        public bool IsActive { get; set; }

        public ReturnPageQuery ReturnPageQuery { get; set; }
    }
}
