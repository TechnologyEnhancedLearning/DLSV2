namespace DigitalLearningSolutions.Data.Models.DelegateUpload
{
    using System.Collections.Generic;

    public class BulkUploadResult
    {
        public enum ErrorReasons
        {
            InvalidJobGroupId,
            InvalidLastName,
            InvalidFirstName,
            InvalidActive,
            NoRecordForDelegateId,
            UnexpectedErrorForUpdate,
            UnexpectedErrorForCreate,
            ParameterError,
            AliasIdInUse,
            EmailAddressInUse
        }

        public BulkUploadResult(
            int processed,
            int registered,
            int updated,
            int skipped,
            IEnumerable<(int RowNumber, ErrorReasons Reason)> errors
        )
        {
            Processed = processed;
            Registered = registered;
            Updated = updated;
            Skipped = skipped;
            Errors = errors;
        }

        public IEnumerable<(int RowNumber, ErrorReasons Reason)> Errors { get; set; }
        public int Processed { get; set; }
        public int Registered { get; set; }
        public int Updated { get; set; }
        public int Skipped { get; set; }
    }
}
