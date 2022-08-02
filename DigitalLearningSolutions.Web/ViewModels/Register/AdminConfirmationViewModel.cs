namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;

    public class AdminConfirmationViewModel
    {
        public AdminConfirmationViewModel(
            bool primaryEmailIsUnverified,
            List<(string centreName, string unverifiedEmail)> unverifiedCentreEmails
        )
        {
            PrimaryEmailIsUnverified = primaryEmailIsUnverified;
            UnverifiedCentreEmails = unverifiedCentreEmails;
        }

        public bool PrimaryEmailIsUnverified { get; }
        public List<(string centreName, string unverifiedEmail)> UnverifiedCentreEmails { get; }
    }
}
