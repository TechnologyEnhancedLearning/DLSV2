namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class InternalConfirmationViewModel
    {
        public InternalConfirmationViewModel(
            string candidateNumber,
            bool approved,
            bool hasAdminAccountAtCentre,
            int? centreId,
            string? centreEmailIfUnverified,
            string centreName,
            bool hasDelegateAccountAtAdminCentre = false
        )
        {
            CandidateNumber = candidateNumber;
            Approved = approved;
            HasAdminAccountAtCentre = hasAdminAccountAtCentre;
            CentreId = centreId;
            CentreEmailIfUnverified = centreEmailIfUnverified;
            CentreName = centreName;
            HasDelegateAccountAtAdminCentre = hasDelegateAccountAtAdminCentre;
        }

        public string CandidateNumber { get; }
        public bool Approved { get; }
        public bool HasAdminAccountAtCentre { get; }
        public int? CentreId { get; }
        public string? CentreEmailIfUnverified { get; }
        public string CentreName { get; }
        public bool HasDelegateAccountAtAdminCentre { get; }
    }
}
