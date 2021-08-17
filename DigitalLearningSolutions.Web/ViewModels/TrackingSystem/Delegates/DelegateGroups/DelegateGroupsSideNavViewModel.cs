namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class DelegateGroupsSideNavViewModel
    {
        public DelegateGroupsSideNavViewModel(string groupName, DelegateGroupPage currentPage)
        {
            GroupName = groupName;
            CurrentPage = currentPage;
        }

        public string GroupName { get; set; }

        public DelegateGroupPage CurrentPage { get; set; }
    }
}
