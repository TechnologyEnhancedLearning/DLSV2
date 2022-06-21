namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class InternalConfirmationViewModel
    {
        public InternalConfirmationViewModel(string candidateNumber, bool approved, bool hasAdminAccountAtCentre, int? centreId)
        {
            CandidateNumber = candidateNumber;
            Approved = approved;
            HasAdminAccountAtCentre = hasAdminAccountAtCentre;
            CentreId = centreId;
        }

        public string CandidateNumber { get; }
        public bool Approved { get; }
        public bool HasAdminAccountAtCentre { get; }
        public int? CentreId { get; }
    }
}
