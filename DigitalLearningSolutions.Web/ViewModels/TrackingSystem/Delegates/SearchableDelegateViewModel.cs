namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableDelegateViewModel : BaseFilterableViewModel
    {
        public SearchableDelegateViewModel(
            DelegateUserCard delegateUser,
            IEnumerable<CustomFieldViewModel> customFields
        )
        {
            DelegateInfo = new DelegateInfoViewModel(delegateUser, customFields.ToList());
            Tags = FilterableTagHelper.GetCurrentTagsForDelegateUser(delegateUser);
        }

        public DelegateInfoViewModel DelegateInfo { get; set; }

        public string RegStatusTagName =>
            DelegateInfo.IsSelfReg
                ? "Self registered" + (DelegateInfo.IsExternalReg ? " (External)" : "")
                : "Registered by centre";
    }
}
