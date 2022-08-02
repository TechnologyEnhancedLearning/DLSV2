namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using System.Linq;

    public class InternalConfirmationViewModel
    {
        public InternalConfirmationViewModel(
            string candidateNumber,
            bool approved,
            bool hasAdminAccountAtCentre,
            int? centreId,
            bool primaryEmailIsUnverified,
            List<(int centreId, string centreName, string? centreSpecificEmail)> unverifiedCentreEmails
        )
        {
            CandidateNumber = candidateNumber;
            Approved = approved;
            HasAdminAccountAtCentre = hasAdminAccountAtCentre;
            CentreId = centreId;
            PrimaryEmailIsUnverified = primaryEmailIsUnverified;
            UnverifiedCentreEmails =
                unverifiedCentreEmails.Select(uce => (uce.centreName, uce.centreSpecificEmail)).ToList();
        }

        public string CandidateNumber { get; }
        public bool Approved { get; }
        public bool HasAdminAccountAtCentre { get; }
        public int? CentreId { get; }
        public bool PrimaryEmailIsUnverified { get; }
        public List<(string centreName, string? centreSpecificEmail)> UnverifiedCentreEmails { get; }
    }
}
