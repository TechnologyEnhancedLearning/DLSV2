namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;

    public class UnverifiedEmailListViewModel
    {
        public readonly bool AtLeastOneCentreEmailIsUnverified;
        public readonly string? PrimaryEmailIfUnverified;
        public readonly bool PrimaryEmailIsVerified;
        public readonly Dictionary<string, List<string>> UnverifiedCentreEmailsDifferentFromPrimaryEmail;

        public UnverifiedEmailListViewModel(
            string? primaryEmailIfUnverified,
            Dictionary<string, List<string>> unverifiedCentreEmails
        )
        {
            PrimaryEmailIfUnverified = primaryEmailIfUnverified;
            PrimaryEmailIsVerified = string.IsNullOrWhiteSpace(primaryEmailIfUnverified);
            AtLeastOneCentreEmailIsUnverified = unverifiedCentreEmails.Any();

            CentresWhereUnverifiedCentreEmailIsSameAsPrimaryEmail =
                !PrimaryEmailIsVerified && unverifiedCentreEmails.ContainsKey(PrimaryEmailIfUnverified!)
                    ? unverifiedCentreEmails[PrimaryEmailIfUnverified]
                    : null;
            PrimaryEmailIsUnverifiedAndTheSameAsAnUnverifiedCentreEmail =
                !PrimaryEmailIsVerified && CentresWhereUnverifiedCentreEmailIsSameAsPrimaryEmail != null;
            UnverifiedCentreEmailsDifferentFromPrimaryEmail =
                GetUnverifiedCentreEmailsDifferentFromPrimaryEmail(unverifiedCentreEmails);
        }

        public List<string>? CentresWhereUnverifiedCentreEmailIsSameAsPrimaryEmail { get; set; }
        public bool PrimaryEmailIsUnverifiedAndTheSameAsAnUnverifiedCentreEmail { get; set; }

        private Dictionary<string, List<string>> GetUnverifiedCentreEmailsDifferentFromPrimaryEmail(
            Dictionary<string, List<string>> unverifiedCentreEmails
        )
        {
            if (PrimaryEmailIsUnverifiedAndTheSameAsAnUnverifiedCentreEmail)
            {
                unverifiedCentreEmails.Remove(PrimaryEmailIfUnverified!);
                return unverifiedCentreEmails;
            }

            return unverifiedCentreEmails;
        }
    }
}
