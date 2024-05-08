namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.BulkUpload
{
    public class AddWhoToGroupViewModel
    {
        public AddWhoToGroupViewModel() { }
        public AddWhoToGroupViewModel(
            string groupName,
            bool includeUpdatedDelegates,
            int toUpdateActiveCount,
            int toRegisterActiveCount
            )
        {
            GroupName = groupName;
            IncludeUpdatedDelegates = includeUpdatedDelegates;
            ToUpdateActiveCount = toUpdateActiveCount;
            ToRegisterActiveCount = toRegisterActiveCount;
        }
        public string? GroupName { get; set; }
        public bool IncludeUpdatedDelegates { get; set; }
        public int ToUpdateActiveCount { get; set; }
        public int ToRegisterActiveCount { get; set; }
    }
}
