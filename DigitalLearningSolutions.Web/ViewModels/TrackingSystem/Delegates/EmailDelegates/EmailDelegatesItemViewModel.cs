namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using DigitalLearningSolutions.Data.Models.User;

    public class EmailDelegatesItemViewModel

    {
        public EmailDelegatesItemViewModel(DelegateUserCard delegateUser)
        {
            Id = delegateUser.Id;
            Name = delegateUser.SearchableName;
            Email = delegateUser.EmailAddress;
            if (delegateUser.DateRegistered.HasValue)
            {
                RegistrationDate = delegateUser.DateRegistered.Value.ToString("dd/MM/yyyy");
            }

            SendWelcomeEmail = false;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? RegistrationDate { get; set; }
        public bool SendWelcomeEmail { get; set; }
    }
}
