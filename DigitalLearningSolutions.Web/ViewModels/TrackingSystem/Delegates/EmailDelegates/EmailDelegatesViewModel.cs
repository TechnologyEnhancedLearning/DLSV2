namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;

    public class EmailDelegatesViewModel
    {
        public EmailDelegatesViewModel() { }

        public EmailDelegatesViewModel(IEnumerable<DelegateUserCard> delegateUsers)
        {
            SetDelegates(delegateUsers);
        }

        public void SetDelegates(IEnumerable<DelegateUserCard> delegateUsers)
        {
            Delegates = delegateUsers.Select(delegateUser => new EmailDelegatesItemViewModel(delegateUser));
        }

        public IEnumerable<EmailDelegatesItemViewModel>? Delegates { get; set; }

        [Required(ErrorMessage = "You must select at least one delegate")]
        public IEnumerable<int>? SelectedDelegateIds { get; set; }
    }
}
