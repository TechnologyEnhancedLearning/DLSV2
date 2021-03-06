﻿namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(string candidateNumber, bool approved, int? centreId)
        {
            CandidateNumber = candidateNumber;
            Approved = approved;
            CentreId = centreId;
        }

        public string CandidateNumber { get; set; }
        public bool Approved { get; set; }
        public int? CentreId { get; set; }
    }
}
