namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Data.Models.User;

    public class EmailDelegatesItemViewModel
    {
        public EmailDelegatesItemViewModel(DelegateUserCard delegateUser, bool preChecked = false)
        {
            Id = delegateUser.Id;
            Name = delegateUser.SearchableName;
            Email = delegateUser.EmailAddress;
            if (delegateUser.DateRegistered.HasValue)
            {
                RegistrationDate = delegateUser.DateRegistered.Value.ToString(DateHelper.StandardDateFormat);
            }

            PreChecked = preChecked;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? RegistrationDate { get; set; }
        public bool PreChecked { get; set; }
    }
}
