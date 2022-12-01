namespace DigitalLearningSolutions.Web.ViewModels.VerifyEmail
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;

    public class VerifyYourEmailViewModel
    {
        public VerifyYourEmailViewModel(
            EmailVerificationReason emailVerificationReason,
            string? primaryEmail,
            IReadOnlyCollection<(int centreId, string centreName, string centreEmail)> centreSpecificEmails
        )
        {
            EmailVerificationReason = emailVerificationReason;
            PrimaryEmail = primaryEmail;
            CentreSpecificEmails = centreSpecificEmails;
            DistinctUnverifiedEmailsCount = GetDistinctUnverifiedEmailsCount(primaryEmail, centreSpecificEmails);
            CentreEmailsExcludingFirstParagraph =
                primaryEmail == null ? centreSpecificEmails.Skip(1) : centreSpecificEmails;
        }

        public EmailVerificationReason EmailVerificationReason { get; set; }
        public string? PrimaryEmail { get; set; }
        public IEnumerable<(int centreId, string centreName, string centreEmail)> CentreSpecificEmails { get; set; }
        public int DistinctUnverifiedEmailsCount { get; set; }
        public bool SingleUnverifiedEmail => DistinctUnverifiedEmailsCount == 1;

        public IEnumerable<(int centreId, string centreName, string centreEmail)> CentreEmailsExcludingFirstParagraph
        {
            get;
            set;
        }

        private static int GetDistinctUnverifiedEmailsCount(
            string? primaryEmail,
            IEnumerable<(int centreId, string centreName, string centreEmail)> centreSpecificEmails
        )
        {
            var unverifiedEmailsList = centreSpecificEmails.Select(cse => cse.centreEmail).ToList();
            if (primaryEmail != null)
            {
                unverifiedEmailsList.Add(primaryEmail);
            }

            return unverifiedEmailsList.Distinct().Count();
        }
    }
}
