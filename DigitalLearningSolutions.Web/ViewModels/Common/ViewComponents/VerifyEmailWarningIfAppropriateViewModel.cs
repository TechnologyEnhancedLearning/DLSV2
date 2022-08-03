namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;

    public class VerifyEmailWarningIfAppropriateViewModel
    {
        public readonly bool AtLeastOneCentreEmailIsUnverified;
        public readonly bool IsChooseACentrePage;
        public readonly bool IsMyAccountPage;
        public readonly bool IsRegistrationJourney;
        public readonly bool MultipleCentreEmailsAreUnverified;
        public readonly bool MultipleEmailsAreUnverified;
        public readonly bool PrimaryEmailIsUnverified;
        public readonly List<(string centreName, string unverifiedEmail)> UnverifiedCentreEmails;
        public readonly string? UnverifiedPrimaryEmail;

        public VerifyEmailWarningIfAppropriateViewModel(
            bool isChooseACentrePage,
            bool isMyAccountPage,
            bool isRegistrationJourney,
            string? unverifiedPrimaryEmail,
            List<(string centreName, string unverifiedEmail)> unverifiedCentreEmails
        )
        {
            IsChooseACentrePage = isChooseACentrePage;
            IsMyAccountPage = isMyAccountPage;
            IsRegistrationJourney = isRegistrationJourney;
            UnverifiedPrimaryEmail = unverifiedPrimaryEmail;
            UnverifiedCentreEmails = unverifiedCentreEmails;
            PrimaryEmailIsUnverified = !string.IsNullOrWhiteSpace(unverifiedPrimaryEmail);
            AtLeastOneCentreEmailIsUnverified = UnverifiedCentreEmails.Any();
            MultipleCentreEmailsAreUnverified = UnverifiedCentreEmails.Count > 1;
            MultipleEmailsAreUnverified = UnverifiedCentreEmails.Count + (PrimaryEmailIsUnverified ? 1 : 0) > 1;
        }
    }
}
