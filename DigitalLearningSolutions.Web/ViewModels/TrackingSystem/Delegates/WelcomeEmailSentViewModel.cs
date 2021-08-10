namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Models.User;

    public class WelcomeEmailSentViewModel
    {
        public WelcomeEmailSentViewModel() { }

        public WelcomeEmailSentViewModel(DelegateUserCard delegateUser)
        {
            Name = delegateUser.SearchableName;
            CandidateNumber = delegateUser.CandidateNumber;
        }
        
        public string Name { get; set; }
        public string CandidateNumber { get; set; }
    }
}
