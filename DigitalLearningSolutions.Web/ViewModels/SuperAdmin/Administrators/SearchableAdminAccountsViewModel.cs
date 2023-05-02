namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Administrators
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableAdminAccountsViewModel : BaseFilterableViewModel
    {
        public readonly bool CanShowDeleteAdminButton;
        public readonly bool CanShowDeactivateAdminButton;

        public SearchableAdminAccountsViewModel(
            AdminEntity admin,
            AdminAccount loggedInSuperAdminAccount,
            ReturnPageQuery returnPageQuery
        )
        {
            Id = admin.AdminAccount.Id;
            Name = admin.UserAccount?.FirstName + " " + admin.UserAccount?.LastName;
            FirstName = admin.UserAccount?.FirstName;
            LastName = admin.UserAccount?.LastName;
            PrimaryEmail = admin.UserAccount?.PrimaryEmail;
            UserAccountID = admin.AdminAccount.UserId;
            Centre = admin.Centre?.CentreName;
            CentreEmail = admin.UserCentreDetails?.Email;
            IsLocked = admin.UserAccount?.FailedLoginCount >= AuthHelper.FailedLoginThreshold;
            IsAdminActive = admin.AdminAccount.Active;
            IsUserActive = admin.UserAccount.Active;

            CanShowDeactivateAdminButton = IsAdminActive && admin.AdminIdReferenceCount > 0;
            CanShowDeleteAdminButton = admin.AdminIdReferenceCount == 0;

            Tags = FilterableTagHelper.GetCurrentTagsForAdmin(admin);
            ReturnPageQuery = returnPageQuery;
        }

        public int Id { get; set; }        
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrimaryEmail { get; set; }
        public int UserAccountID { get; set; }
        public string Centre { get; set; }
        public string CentreEmail { get; set; }
        public bool IsLocked { get; set; }
        public bool IsAdminActive { get; set; }
        public bool IsUserActive { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }
    }
}
