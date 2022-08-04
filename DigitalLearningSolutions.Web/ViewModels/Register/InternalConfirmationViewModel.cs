namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class InternalConfirmationViewModel
    {
        public InternalConfirmationViewModel(
            string candidateNumber,
            bool approved,
            bool hasAdminAccountAtCentre,
            int? centreId,
            string? unverifiedPrimaryEmail,
            string? unverifiedCentreEmail,
            string centreName
        )
        {
            CandidateNumber = candidateNumber;
            Approved = approved;
            HasAdminAccountAtCentre = hasAdminAccountAtCentre;
            CentreId = centreId;
            UnverifiedPrimaryEmail = unverifiedPrimaryEmail;
            UnverifiedCentreEmail = unverifiedCentreEmail;
            CentreName = centreName;
        }

        public string CandidateNumber { get; }
        public bool Approved { get; }
        public bool HasAdminAccountAtCentre { get; }
        public int? CentreId { get; }
        public string? UnverifiedPrimaryEmail { get; }
        public string? UnverifiedCentreEmail { get; }
        public string CentreName { get; }
    }
}
