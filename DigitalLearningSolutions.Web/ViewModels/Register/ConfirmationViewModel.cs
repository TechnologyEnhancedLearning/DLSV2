namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(
            string candidateNumber,
            bool approved,
            int? centreId,
            string? unverifiedPrimaryEmail,
            string? unverifiedCentreEmail,
            string centreName
        )
        {
            CandidateNumber = candidateNumber;
            Approved = approved;
            CentreId = centreId;
            UnverifiedPrimaryEmail = unverifiedPrimaryEmail;
            UnverifiedCentreEmail = unverifiedCentreEmail;
            CentreName = centreName;
        }

        public string CandidateNumber { get; set; }
        public bool Approved { get; set; }
        public int? CentreId { get; set; }
        public string? UnverifiedPrimaryEmail { get; }
        public string? UnverifiedCentreEmail { get; }
        public string CentreName { get; }
    }
}
