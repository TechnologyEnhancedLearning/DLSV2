namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;

    public class BulkUploadResultsViewModel
    {
        public BulkUploadResultsViewModel(BulkUploadResult bulkUploadResult)
        {
            Processed = bulkUploadResult.Processed;
            Registered = bulkUploadResult.Registered;
            Updated = bulkUploadResult.Updated;
            Skipped = bulkUploadResult.Skipped;
            Errors = bulkUploadResult.Errors.Select(x => (x.RowNumber, MapReasonToErrorMessage(x.Reason)));
        }

        public IEnumerable<(int RowNumber, string ErrorMessage)> Errors { get; set; }
        public int ErrorCount => Errors.Count();
        public int Processed { get; set; }
        public int Registered { get; set; }
        public int Updated { get; set; }
        public int Skipped { get; set; }

        private string MapReasonToErrorMessage(BulkUploadResult.ErrorReasons reason)
        {
            return reason.ToString();
        }
    }
}
