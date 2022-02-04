namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class DelegateGroupsSideNavViewModel
    {
        public DelegateGroupsSideNavViewModel(int groupId, string groupName, DelegateGroupPage currentPage)
        {
            GroupId = groupId;
            GroupName = groupName;
            CurrentPage = currentPage;
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public DelegateGroupPage CurrentPage { get; set; }
    }
}
