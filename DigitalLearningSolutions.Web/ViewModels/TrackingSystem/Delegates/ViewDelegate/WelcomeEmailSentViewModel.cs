namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;

    public class WelcomeEmailSentViewModel
    {
        public WelcomeEmailSentViewModel() { }

        public WelcomeEmailSentViewModel(DelegateUserCard delegateUser)
        {
            Id = delegateUser.Id;
            Name = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(delegateUser.FirstName, delegateUser.LastName);
            CandidateNumber = delegateUser.CandidateNumber;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string CandidateNumber { get; set; }
    }
}
