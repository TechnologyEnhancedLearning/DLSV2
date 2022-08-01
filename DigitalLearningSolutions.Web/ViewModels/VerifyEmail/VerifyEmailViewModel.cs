namespace DigitalLearningSolutions.Web.ViewModels.VerifyEmail
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;

    public class VerifyEmailViewModel
    {
        public const string CentreEmailExplanation =
            "You will not be able to access that account until you verify the address.";

        public VerifyEmailViewModel(
            EmailVerificationReason emailVerificationReason,
            string? primaryEmail,
            IReadOnlyCollection<(int centreId, string centreName, string centreEmail)> centreSpecificEmails
        )
        {
            EmailVerificationReason = emailVerificationReason;
            PrimaryEmail = primaryEmail;
            CentreSpecificEmails = centreSpecificEmails;
            UnverifiedEmailsCount = centreSpecificEmails.Count + (primaryEmail == null ? 0 : 1);
            SingleUnverifiedEmail = UnverifiedEmailsCount == 1;
            CentreEmailsExcludingFirstParagraph =
                primaryEmail == null ? centreSpecificEmails.Skip(1) : centreSpecificEmails;
        }

        public EmailVerificationReason EmailVerificationReason { get; set; }
        public string? PrimaryEmail { get; set; }
        public IEnumerable<(int centreId, string centreName, string centreEmail)> CentreSpecificEmails { get; set; }
        public int UnverifiedEmailsCount { get; set; }
        public bool SingleUnverifiedEmail { get; set; }

        public IEnumerable<(int centreId, string centreName, string centreEmail)> CentreEmailsExcludingFirstParagraph
        {
            get;
            set;
        }
    }
}
