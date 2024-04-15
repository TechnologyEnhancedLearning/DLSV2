﻿namespace DigitalLearningSolutions.Data.Models.DelegateUpload
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;

    public enum RowStatus
    {
        NotYetProcessed,
        Skipped,
        Registered,
        Updated,
    }

    public class DelegateTableRow
    {
        private static readonly Regex PrnRegex = new Regex(@"^[a-z\d-]+$", RegexOptions.IgnoreCase);

        public DelegateTableRow(IXLTable table, IXLRangeRow row)
        {
            string? FindFieldValue(string name)
            {
                var colNumber = table.FindColumn(col => col.FirstCell().Value.ToString() == name).ColumnNumber();
                return row.Cell(colNumber).GetValue<string?>();
            }

            string? FindNullableFieldValue(string name)
            {
                var cellValue = FindFieldValue(name);
                return !string.IsNullOrWhiteSpace(cellValue) ? cellValue : null;
            }

            RowNumber = row.RowNumber();
            CandidateNumber = FindFieldValue("DelegateID");
            LastName = FindFieldValue("LastName");
            FirstName = FindFieldValue("FirstName");
            JobGroupId = int.TryParse(FindFieldValue("JobGroupID"), out var jobGroupId) ? jobGroupId : (int?)null;
            Active = bool.TryParse(FindFieldValue("Active"), out var active) ? active : (bool?)null;
            Answer1 = FindFieldValue("Answer1");
            Answer2 = FindFieldValue("Answer2");
            Answer3 = FindFieldValue("Answer3");
            Answer4 = FindFieldValue("Answer4");
            Answer5 = FindFieldValue("Answer5");
            Answer6 = FindFieldValue("Answer6");
            Email = FindFieldValue("EmailAddress")?.Trim();
            HasPrnRawValue = FindFieldValue("HasPRN");
            HasPrn = bool.TryParse(HasPrnRawValue, out var hasPrn) ? hasPrn : (bool?)null;
            Prn = FindNullableFieldValue("PRN");
            RowStatus = RowStatus.NotYetProcessed;
        }

        public int RowNumber { get; }
        public string? CandidateNumber { get; }
        public string? FirstName { get; }
        public string? LastName { get; }
        public int? JobGroupId { get; }
        public bool? Active { get; }
        public string? Answer1 { get; }
        public string? Answer2 { get; }
        public string? Answer3 { get; }
        public string? Answer4 { get; }
        public string? Answer5 { get; }
        public string? Answer6 { get; }
        public string? Email { get; }
        private string? HasPrnRawValue { get; }
        public bool? HasPrn { get; }
        public string? Prn { get; }

        public BulkUploadResult.ErrorReason? Error { get; set; }
        public RowStatus RowStatus { get; set; }

        public bool Validate(IEnumerable<int> allowedJobGroupIds)
        {
            if (!JobGroupId.HasValue || !allowedJobGroupIds.Contains(JobGroupId.Value))
            {
                Error = BulkUploadResult.ErrorReason.InvalidJobGroupId;
            }
            else if (string.IsNullOrEmpty(LastName))
            {
                Error = BulkUploadResult.ErrorReason.MissingLastName;
            }
            else if (string.IsNullOrEmpty(FirstName))
            {
                Error = BulkUploadResult.ErrorReason.MissingFirstName;
            }
            else if (!Active.HasValue)
            {
                Error = BulkUploadResult.ErrorReason.InvalidActive;
            }
            else if (string.IsNullOrEmpty(Email) && Active == true && !string.IsNullOrEmpty(CandidateNumber))
            {
                Error = BulkUploadResult.ErrorReason.MissingEmail;
            }
            else if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(CandidateNumber))
            {
                Error = BulkUploadResult.ErrorReason.MissingEmail;
            }
            else if (FirstName.Length > 250)
            {
                Error = BulkUploadResult.ErrorReason.TooLongFirstName;
            }
            else if (LastName.Length > 250)
            {
                Error = BulkUploadResult.ErrorReason.TooLongLastName;
            }
            else if (Answer1 != null && Answer1.Length > 100)
            {
                Error = BulkUploadResult.ErrorReason.TooLongAnswer1;
            }
            else if (Answer2 != null && Answer2.Length > 100)
            {
                Error = BulkUploadResult.ErrorReason.TooLongAnswer2;
            }
            else if (Answer3 != null && Answer3.Length > 100)
            {
                Error = BulkUploadResult.ErrorReason.TooLongAnswer3;
            }
            else if (Answer4 != null && Answer4.Length > 100)
            {
                Error = BulkUploadResult.ErrorReason.TooLongAnswer4;
            }
            else if (Answer5 != null && Answer5.Length > 100)
            {
                Error = BulkUploadResult.ErrorReason.TooLongAnswer5;
            }
            else if (Answer6 != null && Answer6.Length > 100)
            {
                Error = BulkUploadResult.ErrorReason.TooLongAnswer6;
            }
            else if (!string.IsNullOrWhiteSpace(HasPrnRawValue) && !bool.TryParse(HasPrnRawValue, out _))
            {
                Error = BulkUploadResult.ErrorReason.InvalidHasPrnValue;
            }
            else if (HasPrn.HasValue && HasPrn.Value && string.IsNullOrEmpty(Prn))
            {
                Error = BulkUploadResult.ErrorReason.HasPrnButMissingPrnValue;
            }
            else if (HasPrn.HasValue && !HasPrn.Value && !string.IsNullOrEmpty(Prn))
            {
                Error = BulkUploadResult.ErrorReason.PrnButHasPrnIsFalse;
            }
            else if (!string.IsNullOrEmpty(Prn) && (Prn.Length < 5 || Prn.Length > 20))
            {
                Error = BulkUploadResult.ErrorReason.InvalidPrnLength;
            }
            else if (!string.IsNullOrEmpty(Prn) && !PrnRegex.IsMatch(Prn))
            {
                Error = BulkUploadResult.ErrorReason.InvalidPrnCharacters;
            }
            else if (!string.IsNullOrEmpty(Email))
            {
                if (!new EmailAddressAttribute().IsValid(Email))
                {
                    Error = BulkUploadResult.ErrorReason.BadFormatEmail;
                }
                else if (Email.Length > 250)
                {
                    Error = BulkUploadResult.ErrorReason.TooLongEmail;
                }
                else if (Email.Any(char.IsWhiteSpace))
                {
                    Error = BulkUploadResult.ErrorReason.WhitespaceInEmail;
                }
            }
            return !Error.HasValue;
        }

        public bool MatchesDelegateEntity(DelegateEntity delegateEntity)
        {
            if (delegateEntity.UserAccount.FirstName != FirstName)
            {
                return false;
            }

            if (delegateEntity.UserAccount.LastName != LastName)
            {
                return false;
            }

            if (delegateEntity.UserAccount.JobGroupId != JobGroupId!.Value)
            {
                return false;
            }

            if (delegateEntity.DelegateAccount.Active != Active!.Value)
            {
                return false;
            }

            if ((delegateEntity.DelegateAccount.Answer1 ?? string.Empty) != Answer1)
            {
                return false;
            }

            if ((delegateEntity.DelegateAccount.Answer2 ?? string.Empty) != Answer2)
            {
                return false;
            }

            if ((delegateEntity.DelegateAccount.Answer3 ?? string.Empty) != Answer3)
            {
                return false;
            }

            if ((delegateEntity.DelegateAccount.Answer4 ?? string.Empty) != Answer4)
            {
                return false;
            }

            if ((delegateEntity.DelegateAccount.Answer5 ?? string.Empty) != Answer5)
            {
                return false;
            }

            if ((delegateEntity.DelegateAccount.Answer6 ?? string.Empty) != Answer6)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(Email) && !new EmailAddressAttribute().IsValid(delegateEntity.EmailForCentreNotifications) | delegateEntity.EmailForCentreNotifications != Email)
            {
                return false;
            }

            if (delegateEntity.UserAccount.ProfessionalRegistrationNumber != Prn)
            {
                return false;
            }

            var userHasPrn = PrnHelper.GetHasPrnForDelegate(
                delegateEntity.UserAccount.HasBeenPromptedForPrn,
                delegateEntity.UserAccount.ProfessionalRegistrationNumber
            );

            return userHasPrn == HasPrn || HasPrn == null;
        }
    }
}
