﻿namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;

    public class VerifyEmailWarningIfAppropriateViewModel
    {
        public readonly bool AtLeastOneCentreEmailIsUnverified;
        public readonly bool IsOnMyAccountPage;
        public readonly bool MultipleCentreEmailsAreUnverified;
        public readonly bool MultipleEmailsAreUnverified;
        public readonly bool PrimaryEmailIsUnverified;
        public readonly List<(string centreName, string unverifiedEmail)> UnverifiedCentreEmails;

        public VerifyEmailWarningIfAppropriateViewModel(
            bool isOnMyAccountPage,
            bool primaryEmailIsUnverified,
            List<(string centreName, string unverifiedEmail)> unverifiedCentreEmails
        )
        {
            IsOnMyAccountPage = isOnMyAccountPage;
            PrimaryEmailIsUnverified = primaryEmailIsUnverified;
            UnverifiedCentreEmails = unverifiedCentreEmails;
            AtLeastOneCentreEmailIsUnverified = UnverifiedCentreEmails.Any();
            MultipleCentreEmailsAreUnverified = UnverifiedCentreEmails.Count > 1;
            MultipleEmailsAreUnverified = UnverifiedCentreEmails.Count + (PrimaryEmailIsUnverified ? 1 : 0) > 1;
        }
    }
}
