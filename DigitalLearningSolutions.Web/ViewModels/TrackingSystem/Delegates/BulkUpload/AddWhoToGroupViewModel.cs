namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.BulkUpload
{
    public class AddWhoToGroupViewModel
    {
        public AddWhoToGroupViewModel() { }
        public AddWhoToGroupViewModel(
            string groupName,
            bool includeUpdatedDelegates,
            int toProcessCount,
            int toRegisterCount
            )
        {
            GroupName = groupName;
            IncludeUpdatedDelegates = includeUpdatedDelegates;
            ToProcessCount = toProcessCount;
            ToRegisterCount = toRegisterCount;
        }
        public string? GroupName { get; set; }
        public bool IncludeUpdatedDelegates { get; set; }
        public int ToProcessCount { get; set; }
        public int ToRegisterCount { get; set; }
    }
}
