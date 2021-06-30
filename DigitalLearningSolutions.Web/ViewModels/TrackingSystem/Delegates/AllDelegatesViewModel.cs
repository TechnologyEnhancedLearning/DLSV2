namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;

    public class AllDelegatesViewModel
    {
        public AllDelegatesViewModel(IEnumerable<DelegateUser> delegateUsers)
        {
            Delegates = delegateUsers.Select(delegateUser => new SearchableDelegateViewModel(delegateUser));
        }

        public IEnumerable<SearchableDelegateViewModel> Delegates { get; }
    }
}
