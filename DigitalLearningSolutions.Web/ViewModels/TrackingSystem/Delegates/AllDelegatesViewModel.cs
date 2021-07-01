namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;

    public class AllDelegatesViewModel
    {
        public AllDelegatesViewModel(
            int centreId,
            IEnumerable<DelegateUserCard> delegateUsers,
            CustomPromptHelper helper
        )
        {
            CentreId = centreId;
            Delegates = delegateUsers.Select(
                delegateUser =>
                {
                    var customFields = helper.GetCustomFieldViewModelsForCentre(
                        centreId,
                        delegateUser.Answer1,
                        delegateUser.Answer2,
                        delegateUser.Answer3,
                        delegateUser.Answer4,
                        delegateUser.Answer5,
                        delegateUser.Answer6
                    );
                    return new SearchableDelegateViewModel(delegateUser, customFields);
                }
            );
        }

        public int CentreId { get; set; }
        public IEnumerable<SearchableDelegateViewModel> Delegates { get; set; }
    }
}
