namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.BulkUpload
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
        public BulkUploadResultsViewModel(int processedCount, int registeredCount, int updatedCount, int skippedCount, IEnumerable<(int, string)> errors, int day, int month, int year, int totalSteps)
        {
            ProcessedCount = processedCount;
            TotalSteps = totalSteps;
            RegisteredCount = registeredCount;
            UpdatedCount = updatedCount;
            SkippedCount = skippedCount;
            Errors = errors;
            Day = day;
            Month = month;
            Year = year;
        }

        public IEnumerable<(int RowNumber, string ErrorMessage)> Errors { get; set; }
        public int ErrorCount => Errors.Count();
        public int TotalSteps { get; set; }
        public int ProcessedCount { get; set; }
        public int RegisteredCount { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedCount { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        private static string MapReasonToErrorMessage(BulkUploadResult.ErrorReason reason)
        {
            return reason switch
            {
                BulkUploadResult.ErrorReason.InvalidJobGroupId =>
                    "JobGroupID was not valid, please ensure a valid job group is selected from the provided list",
                BulkUploadResult.ErrorReason.MissingLastName =>
                    "LastName was not provided. LastName is a required field and cannot be left blank",
                BulkUploadResult.ErrorReason.MissingFirstName =>
                    "FirstName was not provided. FirstName is a required field and cannot be left blank",
                BulkUploadResult.ErrorReason.MissingEmail =>
                    "EmailAddress was not provided. EmailAddress is a required field and cannot be left blank",
                BulkUploadResult.ErrorReason.InvalidActive =>
                    "Active field could not be read. The Active field should contain 'TRUE' or 'FALSE'",
                BulkUploadResult.ErrorReason.NoRecordForDelegateId =>
                    "No existing delegate record was found with the DelegateID provided",
                BulkUploadResult.ErrorReason.UnexpectedErrorForCreate =>
                    "Unexpected error when creating delegate",
                BulkUploadResult.ErrorReason.UnexpectedErrorForUpdate =>
                    "Unexpected error when updating delegate details",
                BulkUploadResult.ErrorReason.ParameterError => "Parameter error when updating delegate details",
                BulkUploadResult.ErrorReason.EmailAddressInUse =>
                    "The EmailAddress is already in use by another delegate",
                BulkUploadResult.ErrorReason.TooLongFirstName => "FirstName must be 250 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongLastName => "LastName must be 250 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongEmail => "EmailAddress must be 250 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongAnswer1 => "Answer1 must be 100 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongAnswer2 => "Answer2 must be 100 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongAnswer3 => "Answer3 must be 100 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongAnswer4 => "Answer4 must be 100 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongAnswer5 => "Answer5 must be 100 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongAnswer6 => "Answer6 must be 100 characters or fewer",
                BulkUploadResult.ErrorReason.BadFormatEmail =>
                    "EmailAddress must be in the correct format, like name@example.com",
                BulkUploadResult.ErrorReason.WhitespaceInEmail =>
                    "EmailAddress must not contain any whitespace characters",
                BulkUploadResult.ErrorReason.HasPrnButMissingPrnValue =>
                    "HasPRN was set to true, but PRN was not provided. When HasPRN is set to true, PRN is a required field",
                BulkUploadResult.ErrorReason.PrnButHasPrnIsFalse =>
                    "HasPRN was set to false, but PRN was provided. When HasPRN is set to false, PRN is required to be empty",
                BulkUploadResult.ErrorReason.InvalidPrnLength => "PRN must be between 5 and 20 characters",
                BulkUploadResult.ErrorReason.InvalidPrnCharacters =>
                    "Invalid PRN format - Only alphanumeric characters (a-z, A-Z and 0-9) and hyphens (-) allowed",
                BulkUploadResult.ErrorReason.InvalidHasPrnValue => "HasPRN field could not be read. The HasPRN field should contain 'TRUE' or 'FALSE' or be left blank",
                _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null),
            };
        }
    }
}
