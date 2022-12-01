namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(
            string candidateNumber,
            bool approved,
            int? centreId,
            string? primaryEmailIfUnverified,
            string? centreEmailIfUnverified,
            string centreName
        )
        {
            CandidateNumber = candidateNumber;
            Approved = approved;
            CentreId = centreId;
            PrimaryEmailIfUnverified = primaryEmailIfUnverified;
            CentreEmailIfUnverified = centreEmailIfUnverified;
            CentreName = centreName;
        }

        public string CandidateNumber { get; set; }
        public bool Approved { get; set; }
        public int? CentreId { get; set; }
        public string? PrimaryEmailIfUnverified { get; }
        public string? CentreEmailIfUnverified { get; }
        public string CentreName { get; }
    }
}
