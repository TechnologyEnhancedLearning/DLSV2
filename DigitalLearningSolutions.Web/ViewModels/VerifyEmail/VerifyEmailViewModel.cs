namespace DigitalLearningSolutions.Web.ViewModels.VerifyEmail
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;

    public class VerifyEmailViewModel
    {
        public VerifyEmailViewModel(
            EmailVerificationReason emailVerificationReason,
            string? primaryEmail,
            List<(string centreName, string centreEmail)> centreSpecificEmails
        )
        {
            EmailVerificationReason = emailVerificationReason;
            PrimaryEmail = primaryEmail;
            CentreSpecificEmails = centreSpecificEmails;
            UnverifiedEmailsCount = centreSpecificEmails.Count + (primaryEmail == null ? 0 : 1);
        }

        public EmailVerificationReason EmailVerificationReason { get; set; }
        public string? PrimaryEmail { get; set; }
        public IEnumerable<(string centreName, string centreEmail)> CentreSpecificEmails { get; set; }
        public int UnverifiedEmailsCount { get; set; }
    }
}
