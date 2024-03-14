namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Http;

    public class BulkUploadPreProcessViewModel : UploadDelegatesViewModel
    {
        public BulkUploadPreProcessViewModel(BulkUploadResult bulkUploadResult)
        {
            ToProcessCount = bulkUploadResult.ProcessedCount;
            ToRegisterCount = bulkUploadResult.RegisteredCount;
            ToUpdateOrSkipCount = bulkUploadResult.UpdatedCount;
            Errors = bulkUploadResult.Errors.Select(x => (x.RowNumber, MapReasonToErrorMessage(x.Reason)));
        }

        public IEnumerable<(int RowNumber, string ErrorMessage)> Errors { get; set; }
        public int ErrorCount => Errors.Count();
        public int ToProcessCount { get; set; }
        public int ToRegisterCount { get; set; }
        public int ToUpdateOrSkipCount { get; set; }

        private static string MapReasonToErrorMessage(BulkUploadResult.ErrorReason reason)
        {
            return reason switch
            {
                BulkUploadResult.ErrorReason.InvalidJobGroupId =>
                    "Job group is not valid. Please choose a job group from the list provided",
                BulkUploadResult.ErrorReason.MissingLastName =>
                    "LastName is blank. Last name is a required field and cannot be left blank",
                BulkUploadResult.ErrorReason.MissingFirstName =>
                    "FirstName is blank. First name is a required field and cannot be left blank",
                BulkUploadResult.ErrorReason.MissingEmail =>
                    "EmailAddress is blank. Email address is a required field and cannot be left blank",
                BulkUploadResult.ErrorReason.InvalidActive =>
                    "Active field is invalid. The Active field must contain 'TRUE' or 'FALSE'",
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
                    "EmailAddress is not in the correct format. Email must be valid, like name@example.com",
                BulkUploadResult.ErrorReason.WhitespaceInEmail =>
                    "EmailAddress must not contain any whitespace characters",
                BulkUploadResult.ErrorReason.HasPrnButMissingPrnValue =>
                    "HasPRN is set to true, but PRN was not provided. When HasPRN is set to true, PRN is a required field",
                BulkUploadResult.ErrorReason.PrnButHasPrnIsFalse =>
                    "HasPRN is set to false, but PRN was provided. When HasPRN is set to false, PRN is required to be empty",
                BulkUploadResult.ErrorReason.InvalidPrnLength => "PRN must be between 5 and 20 characters",
                BulkUploadResult.ErrorReason.InvalidPrnCharacters =>
                    "Invalid PRN format - Only alphanumeric characters (a-z, A-Z and 0-9) and hyphens (-) allowed",
                BulkUploadResult.ErrorReason.InvalidHasPrnValue => "HasPRN field could not be read. The HasPRN field should contain 'TRUE' or 'FALSE' or be left blank",
                _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null),
            };
        }
    }
}
