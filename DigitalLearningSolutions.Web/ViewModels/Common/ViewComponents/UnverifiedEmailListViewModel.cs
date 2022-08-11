namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;

    public class UnverifiedEmailListViewModel
    {
        public readonly bool AtLeastOneCentreEmailIsUnverified;
        public readonly List<string>? CentresWhereUnverifiedCentreEmailIsSameAsPrimaryEmail;
        public readonly string? PrimaryEmailIfUnverified;
        public readonly bool PrimaryEmailIsUnverifiedAndTheSameAsAnUnverifiedCentreEmail;
        public readonly bool PrimaryEmailIsVerified;
        public readonly Dictionary<string, List<string>> UnverifiedCentreEmails;

        public UnverifiedEmailListViewModel(
            string? primaryEmailIfUnverified,
            Dictionary<string, List<string>> unverifiedCentreEmails
        )
        {
            PrimaryEmailIfUnverified = primaryEmailIfUnverified;
            UnverifiedCentreEmails = unverifiedCentreEmails;
            PrimaryEmailIsVerified = string.IsNullOrWhiteSpace(primaryEmailIfUnverified);
            AtLeastOneCentreEmailIsUnverified = unverifiedCentreEmails.Any();
            CentresWhereUnverifiedCentreEmailIsSameAsPrimaryEmail =
                PrimaryEmailIfUnverified != null && UnverifiedCentreEmails.ContainsKey(PrimaryEmailIfUnverified)
                    ? UnverifiedCentreEmails[PrimaryEmailIfUnverified]
                    : null;
            PrimaryEmailIsUnverifiedAndTheSameAsAnUnverifiedCentreEmail =
                !PrimaryEmailIsVerified && CentresWhereUnverifiedCentreEmailIsSameAsPrimaryEmail != null;
        }
    }
}
