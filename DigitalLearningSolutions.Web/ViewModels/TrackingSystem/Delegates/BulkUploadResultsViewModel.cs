﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;

    public class BulkUploadResultsViewModel
    {
        public BulkUploadResultsViewModel(BulkUploadResult bulkUploadResult)
        {
            ProcessedCount = bulkUploadResult.ProcessedCount;
            RegisteredCount = bulkUploadResult.RegisteredCount;
            UpdatedCount = bulkUploadResult.UpdatedCount;
            SkippedCount = bulkUploadResult.SkippedCount;
            Errors = bulkUploadResult.Errors.Select(x => (x.RowNumber, MapReasonToErrorMessage(x.Reason)));
        }

        public IEnumerable<(int RowNumber, string ErrorMessage)> Errors { get; set; }
        public int ErrorCount => Errors.Count();
        public int ProcessedCount { get; set; }
        public int RegisteredCount { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedCount { get; set; }

        private string MapReasonToErrorMessage(BulkUploadResult.ErrorReason reason)
        {
            return reason switch
            {
                BulkUploadResult.ErrorReason.InvalidJobGroupId =>
                    "Job group ID was not valid. Please ensure a valid Job Group ID number is provided (use the 'Job Groups' worksheet in the downloaded template for a list of valid IDs).",
                BulkUploadResult.ErrorReason.InvalidLastName =>
                    "Last name was not provided. Last name is a required field and cannot be left blank.",
                BulkUploadResult.ErrorReason.InvalidFirstName =>
                    "First name was not provided. First name is a required field and cannot be left blank.",
                BulkUploadResult.ErrorReason.InvalidEmail =>
                    "Email was not provided. Email is a required field and cannot be left blank.",
                BulkUploadResult.ErrorReason.InvalidActive =>
                    "Active field could not be read. The Active field should contain 'TRUE' or 'FALSE'.",
                BulkUploadResult.ErrorReason.NoRecordForDelegateId =>
                    "No existing delegate record was found with the Delegate ID provided.",
                BulkUploadResult.ErrorReason.UnexpectedErrorForCreate =>
                    "Unexpected error when creating delegate.",
                BulkUploadResult.ErrorReason.UnexpectedErrorForUpdate =>
                    "Unexpected error when updating delegate details.",
                BulkUploadResult.ErrorReason.ParameterError => "Parameter error when updating delegate details.",
                BulkUploadResult.ErrorReason.AliasIdInUse => "The Alias ID is already in use by another delegate.",
                BulkUploadResult.ErrorReason.EmailAddressInUse =>
                    "The Email address is already in use by another delegate.",
                _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null)
            };
        }
    }
}
