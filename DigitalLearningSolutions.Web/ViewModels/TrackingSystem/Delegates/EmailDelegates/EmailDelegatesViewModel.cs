namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;

    public class EmailDelegatesViewModel
    {
        public EmailDelegatesViewModel(IEnumerable<DelegateUserCard> delegateUserCards)
        {
            Delegates = delegateUserCards.Select(delegateUser => new EmailDelegatesItemViewModel(delegateUser));
        }

        public IEnumerable<EmailDelegatesItemViewModel> Delegates { get; set; }
    }
}
