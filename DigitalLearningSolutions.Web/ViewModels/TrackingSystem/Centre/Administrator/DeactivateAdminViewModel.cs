namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;

    public class DeactivateAdminViewModel
    {
        public DeactivateAdminViewModel() { }

        public DeactivateAdminViewModel(AdminEntity admin, ReturnPageQuery returnPageQuery)
        {
            FullName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                admin.UserAccount.FirstName,
                admin.UserAccount.LastName
            );
            EmailAddress = admin.UserCentreDetails?.Email ?? admin.UserAccount.PrimaryEmail;
            ReturnPageQuery = returnPageQuery;
        }

        public string FullName { get; set; }
        public string? EmailAddress { get; set; }

        [BooleanMustBeTrue(ErrorMessage = "You must confirm before deactivating this account")]
        public bool Confirm { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }
    }
}
