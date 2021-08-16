namespace DigitalLearningSolutions.Data.Models.DelegateUpload
{
    using System.Collections.Generic;
    using System.Linq;

    public class BulkUploadResult
    {
        public enum ErrorReason
        {
            InvalidJobGroupId,
            InvalidLastName,
            InvalidFirstName,
            InvalidEmail,
            InvalidActive,
            NoRecordForDelegateId,
            UnexpectedErrorForUpdate,
            UnexpectedErrorForCreate,
            ParameterError,
            AliasIdInUse,
            EmailAddressInUse
        }

        public BulkUploadResult(
            IEnumerable<DelegateTableRow> delegateRows
        )
        {
            delegateRows = delegateRows.ToList();
            ProcessedCount = delegateRows.Count();
            RegisteredCount = delegateRows.Count(dr => dr.RowStatus == RowStatus.Registered);
            UpdatedCount = delegateRows.Count(dr => dr.RowStatus == RowStatus.Updated);
            SkippedCount = delegateRows.Count(dr => dr.RowStatus == RowStatus.Skipped);
            Errors = delegateRows.Where(dr => dr.Error.HasValue).Select(dr => (dr.RowNumber, dr.Error!.Value));
        }

        public IEnumerable<(int RowNumber, ErrorReason Reason)> Errors { get; set; }
        public int ProcessedCount { get; set; }
        public int RegisteredCount { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedCount { get; set; }
    }
}
