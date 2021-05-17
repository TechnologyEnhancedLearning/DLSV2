namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(string candidateNumber)
        {
            CandidateNumber = candidateNumber;
        }
        public string CandidateNumber { get; set; }
    }
}
