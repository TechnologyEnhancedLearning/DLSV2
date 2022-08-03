namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;

    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(
            string candidateNumber,
            bool approved,
            int? centreId,
            string? unverifiedPrimaryEmail,
            List<(string centreName, string? centreSpecificEmail)> unverifiedCentreEmails
        )
        {
            CandidateNumber = candidateNumber;
            Approved = approved;
            CentreId = centreId;
            UnverifiedPrimaryEmail = unverifiedPrimaryEmail;
            UnverifiedCentreEmails = unverifiedCentreEmails;
        }

        public string CandidateNumber { get; set; }
        public bool Approved { get; set; }
        public int? CentreId { get; set; }
        public string? UnverifiedPrimaryEmail { get; }
        public List<(string centreName, string? centreSpecificEmail)> UnverifiedCentreEmails { get; }
    }
}
