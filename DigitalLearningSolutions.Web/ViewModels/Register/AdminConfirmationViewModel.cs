namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;

    public class AdminConfirmationViewModel
    {
        public AdminConfirmationViewModel(
            string? unverifiedPrimaryEmail,
            List<(string centreName, string unverifiedEmail)> unverifiedCentreEmails
        )
        {
            UnverifiedPrimaryEmail = unverifiedPrimaryEmail;
            UnverifiedCentreEmails = unverifiedCentreEmails;
        }

        public string? UnverifiedPrimaryEmail { get; }
        public List<(string centreName, string unverifiedEmail)> UnverifiedCentreEmails { get; }
    }
}
