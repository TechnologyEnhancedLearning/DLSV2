namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.BulkUpload
{
    public class ProcessBulkDelegatesViewModel
    {
        public ProcessBulkDelegatesViewModel(
            int stepNumber, int totalSteps, int rowsProcessed, int totalRows, int maxRowsPerStep, int delegatesRegistered, int delegatesUpdated, int rowsSkipped, int errorCount)
        {
            StepNumber = stepNumber;
            TotalSteps = totalSteps;
            RowsProcessed = rowsProcessed;
            TotalRows = totalRows;
            MaxRowsPerStep = maxRowsPerStep;
            DelegatesRegistered = delegatesRegistered;
            DelegatesUpdated = delegatesUpdated;
            RowsSkipped = rowsSkipped;
            ErrorCount = errorCount;
        }
        public int StepNumber { get; set; }
        public int TotalSteps { get; set; }
        public int RowsProcessed { get; set; }
        public int TotalRows { get; set; }
        public int MaxRowsPerStep { get; set; }
        public int DelegatesRegistered { get; set; }
        public int DelegatesUpdated { get; set; }
        public int RowsSkipped { get; set; }
        public int ErrorCount { get; set; }
    }
}
