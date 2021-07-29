namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class DelegateGroupsSideNavViewModel
    {
        public DelegateGroupsSideNavViewModel(string groupName, DelegateGroupPage page)
        {
            GroupName = groupName;
            Page = page;
        }

        public string GroupName { get; set; }

        public DelegateGroupPage Page { get; set; }
    }
}
