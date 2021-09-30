namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public class DeactivateAdminViewModel
    {
        public readonly CheckboxListItemViewModel[] ConfirmCheckBox =
        {
            new CheckboxListItemViewModel(
                nameof(Confirm),
                "I am sure that I wish to deactivate this account. I understand that if the user has never logged in, their admin account be deleted",
                string.Empty
            )
        };

        public DeactivateAdminViewModel() { }

        public DeactivateAdminViewModel(AdminUser user)
        {
            if (user != null)
            {
                UserId = user.Id;
                FullName = user.FullName;
                EmailAddress = user.EmailAddress;
            }
        }

        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public int UserId { get; }
        public bool Confirm { get; set; }
    }
}
