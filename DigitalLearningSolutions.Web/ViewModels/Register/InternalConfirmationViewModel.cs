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

        public string CandidateNumber { get; set; }
        public bool Approved { get; set; }
        public bool HasAdminAccountAtCentre { get; set; }
        public int? CentreId { get; set; }
    }
}
