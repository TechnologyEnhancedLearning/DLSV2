namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;

    public class DeactivateAdminViewModel
    {
        public DeactivateAdminViewModel() { }

        public DeactivateAdminViewModel(AdminUser user)
        {
            UserId = user.Id;
            FullName = user.FullName;
            EmailAddress = user.EmailAddress;
        }

        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public int UserId { get; }

        //[BooleanMustBeTrue(ErrorMessage = "You must confirm before deactivating this account")]
        [RegularExpression("True", ErrorMessage = "You must confirm before deactivating this account")]
        public bool Confirm { get; set; }
    }
}
