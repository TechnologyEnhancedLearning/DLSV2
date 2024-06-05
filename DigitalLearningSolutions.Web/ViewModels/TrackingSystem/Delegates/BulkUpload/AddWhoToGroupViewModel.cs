namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.BulkUpload
{
    public class AddWhoToGroupViewModel
    {
        public AddWhoToGroupViewModel() { }
        public AddWhoToGroupViewModel(
            string groupName,
            bool includeUpdatedDelegates,
            bool includeSkippedDelegates,
            int toUpdateActiveCount,
            int toRegisterActiveCount
            )
        {
            GroupName = groupName;
            AddWhoToGroupOption = (includeUpdatedDelegates&&includeSkippedDelegates? 3 : (includeUpdatedDelegates ? 2 : 1));
            ToUpdateActiveCount = toUpdateActiveCount;
            ToRegisterActiveCount = toRegisterActiveCount;
        }
        public string? GroupName { get; set; }
        public int AddWhoToGroupOption { get; set; }
        public int ToUpdateActiveCount { get; set; }
        public int ToRegisterActiveCount { get; set; }
    }
}
