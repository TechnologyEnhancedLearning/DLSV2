namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;

    public class DeactivateAdminViewModel
    {
        public DeactivateAdminViewModel() { }

        public DeactivateAdminViewModel(AdminUser user, int? returnPage)
        {
            FullName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(user.FirstName, user.LastName);
            EmailAddress = user.EmailAddress;
            ReturnPage = returnPage;
        }

        public string FullName { get; set; }
        public string? EmailAddress { get; set; }

        [BooleanMustBeTrue(ErrorMessage = "You must confirm before deactivating this account")]
        public bool Confirm { get; set; }

        public int? ReturnPage { get; set; }
    }
}
