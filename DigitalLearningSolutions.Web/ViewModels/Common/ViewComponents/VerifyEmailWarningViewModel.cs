namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;

    public class VerifyEmailWarningViewModel
    {
        public readonly bool IsOnMyAccountPage;
        public readonly bool PrimaryEmailIsUnverified;
        public readonly List<string> UnverifiedCentreEmails;

        public VerifyEmailWarningViewModel(
            bool isOnMyAccountPage,
            bool primaryEmailIsUnverified,
            IEnumerable<(int centreId, string centreName, string? centreSpecificEmail)> unverifiedCentreEmails
        )
        {
            IsOnMyAccountPage = isOnMyAccountPage;
            PrimaryEmailIsUnverified = primaryEmailIsUnverified;
            UnverifiedCentreEmails = unverifiedCentreEmails.Select(uce => uce.centreSpecificEmail).ToList();
        }
    }
}
