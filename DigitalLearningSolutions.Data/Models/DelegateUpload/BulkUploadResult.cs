namespace DigitalLearningSolutions.Data.Models.DelegateUpload
{
    using System.Collections.Generic;
    using System.Linq;

    public class BulkUploadResult
    {
        public enum ErrorReason
        {
            InvalidJobGroupId,
            MissingLastName,
            MissingFirstName,
            MissingEmail,
            InvalidActive,
            NoRecordForDelegateId,
            UnexpectedErrorForUpdate,
            UnexpectedErrorForCreate,
            ParameterError,
            EmailAddressInUse,
            TooLongFirstName,
            TooLongLastName,
            TooLongEmail,
            TooLongAnswer1,
            TooLongAnswer2,
            TooLongAnswer3,
            TooLongAnswer4,
            TooLongAnswer5,
            TooLongAnswer6,
            BadFormatEmail,
            WhitespaceInEmail,
            HasPrnButMissingPrnValue,
            PrnButHasPrnIsFalse,
            InvalidPrnLength,
            InvalidPrnCharacters,
            InvalidHasPrnValue
        }

        public BulkUploadResult() { }

        public BulkUploadResult(
            IReadOnlyCollection<DelegateTableRow> delegateRows
        )
        {
            ProcessedCount = delegateRows.Count;
            RegisteredActiveCount = delegateRows.Count(dr => dr.RowStatus == RowStatus.RegisteredActive);
            RegisteredInactiveCount = delegateRows.Count(dr => dr.RowStatus == RowStatus.RegsiteredInactive);
            UpdatedActiveCount = delegateRows.Count(dr => dr.RowStatus == RowStatus.UpdatedActive);
            UpdatedInactiveCount = delegateRows.Count(dr => dr.RowStatus == RowStatus.UpdatedInactive);
            SkippedCount = delegateRows.Count(dr => dr.RowStatus == RowStatus.Skipped);
            Errors = delegateRows.Where(dr => dr.Error.HasValue).Select(dr => (dr.RowNumber, dr.Error!.Value));
        }

        public IEnumerable<(int RowNumber, ErrorReason Reason)>? Errors { get; set; }
        public int ProcessedCount { get; set; }
        public int RegisteredActiveCount { get; set; }
        public int RegisteredInactiveCount { get; set; }
        public int UpdatedActiveCount { get; set; }
        public int UpdatedInactiveCount { get; set; }
        public int SkippedCount { get; set; }
    }
}
