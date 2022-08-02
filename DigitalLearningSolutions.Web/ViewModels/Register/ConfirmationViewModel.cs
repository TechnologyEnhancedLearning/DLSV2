namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;

    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(
            string candidateNumber,
            bool approved,
            int? centreId,
            bool primaryEmailIsUnverified,
            List<(string centreName, string? centreSpecificEmail)> unverifiedCentreEmails
        )
        {
            CandidateNumber = candidateNumber;
            Approved = approved;
            CentreId = centreId;
            PrimaryEmailIsUnverified = primaryEmailIsUnverified;
            UnverifiedCentreEmails = unverifiedCentreEmails;
        }

        public string CandidateNumber { get; set; }
        public bool Approved { get; set; }
        public int? CentreId { get; set; }
        public bool PrimaryEmailIsUnverified { get; }
        public List<(string centreName, string? centreSpecificEmail)> UnverifiedCentreEmails { get; }
    }
}
