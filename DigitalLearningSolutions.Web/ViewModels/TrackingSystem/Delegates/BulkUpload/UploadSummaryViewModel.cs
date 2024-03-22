namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.BulkUpload
{
    public class UploadSummaryViewModel
    {
        public UploadSummaryViewModel() { }
        public UploadSummaryViewModel(
            int toProcessCount,
            int toRegisterCount,
            int toUpdateCount,
            int maxRowsToProcess,
            int addToGroupOption,
            string? groupName,
            int? day,
            int? month,
            int? year,
            bool includeUpdatedDelegates
            )
        {
            ToProcessCount = toProcessCount;
            ToRegisterCount = toRegisterCount;
            ToUpdateCount = toUpdateCount;
            MaxRowsToProcess = maxRowsToProcess;
            AddToGroupOption = addToGroupOption;
            GroupName = groupName;
            Day = day;
            Month = month;
            Year = year;
            IncludeUpdatedDelegates = includeUpdatedDelegates;
        }
        public int ToProcessCount { get; set; }
        public int ToRegisterCount { get; set; }
        public int ToUpdateCount { get; set; }
        public int MaxRowsToProcess { get; set; }
        public int AddToGroupOption { get; set; }
        public string? GroupName { get; set; }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public bool IncludeUpdatedDelegates { get; set; }
    }
}
