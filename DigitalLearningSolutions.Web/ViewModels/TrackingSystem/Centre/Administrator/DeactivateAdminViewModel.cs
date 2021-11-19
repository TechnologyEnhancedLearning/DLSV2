namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;

    public class DeactivateAdminViewModel
    {
        public DeactivateAdminViewModel() { }

        public DeactivateAdminViewModel(AdminUser user)
        {
            FullName = user.FullName;
            EmailAddress = user.EmailAddress;
        }

        public string FullName { get; set; }
        public string? EmailAddress { get; set; }

        [BooleanMustBeTrue(ErrorMessage = "You must confirm before deactivating this account")]
        public bool Confirm { get; set; }
    }
}
