namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;

    public class EmailDelegatesViewModel
    {
        public EmailDelegatesViewModel() : this(new List<DelegateUserCard>()) { }

        public EmailDelegatesViewModel(IEnumerable<DelegateUserCard> delegateUserCards)
        {
            Delegates = delegateUserCards.Select(delegateUser => new EmailDelegatesItemViewModel(delegateUser));
            SelectedDelegateIds = new List<int>();
        }

        public IEnumerable<EmailDelegatesItemViewModel> Delegates { get; set; }
        public IEnumerable<int> SelectedDelegateIds { get; set; }
    }
}
