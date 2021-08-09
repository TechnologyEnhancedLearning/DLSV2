namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;

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
            return reason switch
            {
                BulkUploadResult.ErrorReasons.InvalidJobGroupId =>
                    "Job group ID was not valid. Please ensure a valid Job Group ID number is provided (use the 'Download Job Groups references' option for a list of valid IDs).",
                BulkUploadResult.ErrorReasons.InvalidLastName =>
                    "Last name was not provided. Last name is a required field and cannot be left blank.",
                BulkUploadResult.ErrorReasons.InvalidFirstName =>
                    "First name was not provided. First name is a required field and cannot be left blank.",
                BulkUploadResult.ErrorReasons.InvalidActive =>
                    "Active field could not be read. The Active field should contain 'True' or 'False'.",
                BulkUploadResult.ErrorReasons.NoRecordForDelegateId =>
                    "No existing delegate record was found with the DelegateID provided.",
                BulkUploadResult.ErrorReasons.UnexpectedErrorForCreate =>
                    "Unexpected error when creating Delegate details.",
                BulkUploadResult.ErrorReasons.UnexpectedErrorForUpdate =>
                    "Unexpected error when updating Delegate details.",
                BulkUploadResult.ErrorReasons.ParameterError => "Parameter error when updating Delegate details.",
                BulkUploadResult.ErrorReasons.AliasIdInUse => "The AliasID is already in use by another delegate.",
                BulkUploadResult.ErrorReasons.EmailAddressInUse =>
                    "The Email address is already in use by another delegate.",
                _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null)
            };
        }
    }
}
