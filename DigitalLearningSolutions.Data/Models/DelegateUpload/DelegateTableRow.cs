namespace DigitalLearningSolutions.Data.Models.DelegateUpload
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;

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
            AliasId = FindFieldValue("AliasID");
            Email = FindFieldValue("EmailAddress")?.Trim();
            HasPrn = bool.TryParse(FindFieldValue("HasPRN"), out var hasPrn) ? hasPrn : (bool?)null;
            Prn = FindNullableFieldValue("PRN");
            RowStatus = RowStatus.NotYetProcessed;
        }

        public int RowNumber { get; set; }
        public string? CandidateNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? JobGroupId { get; set; }
        public bool? Active { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Answer6 { get; set; }
        public string? AliasId { get; set; }
        public string? Email { get; set; }
        public bool? HasPrn { get; set; }
        public string? Prn { get; set; }

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
            else if (string.IsNullOrEmpty(Email))
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
            else if (Email.Length > 250)
            {
                Error = BulkUploadResult.ErrorReason.TooLongEmail;
            }
            else if (!new EmailAddressAttribute().IsValid(Email))
            {
                Error = BulkUploadResult.ErrorReason.BadFormatEmail;
            }
            else if (Email.Any(char.IsWhiteSpace))
            {
                Error = BulkUploadResult.ErrorReason.WhitespaceInEmail;
            }
            else if (AliasId != null && AliasId.Length > 250)
            {
                Error = BulkUploadResult.ErrorReason.TooLongAliasId;
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
            else if (HasPrn.HasValue && HasPrn.Value && string.IsNullOrEmpty(Prn))
            {
                Error = BulkUploadResult.ErrorReason.HasPrnButMissingPrnValue;
            }
            else if (!string.IsNullOrEmpty(Prn) && (Prn.Length < 5 || Prn.Length > 20))
            {
                Error = BulkUploadResult.ErrorReason.InvalidPrnLength;
            }
            else if (!string.IsNullOrEmpty(Prn) && !PrnRegex.IsMatch(Prn))
            {
                Error = BulkUploadResult.ErrorReason.InvalidPrnCharacters;
            }

            return !Error.HasValue;
        }

        public bool MatchesDelegateUser(DelegateUser delegateUser)
        {
            if (CandidateNumber != null && (delegateUser.AliasId ?? string.Empty) != AliasId)
            {
                return false;
            }

            if ((delegateUser.FirstName ?? string.Empty) != FirstName)
            {
                return false;
            }

            if (delegateUser.LastName != LastName)
            {
                return false;
            }

            if (delegateUser.JobGroupId != JobGroupId!.Value)
            {
                return false;
            }

            if (delegateUser.Active != Active!.Value)
            {
                return false;
            }

            if ((delegateUser.Answer1 ?? string.Empty) != Answer1)
            {
                return false;
            }

            if ((delegateUser.Answer2 ?? string.Empty) != Answer2)
            {
                return false;
            }

            if ((delegateUser.Answer3 ?? string.Empty) != Answer3)
            {
                return false;
            }

            if ((delegateUser.Answer4 ?? string.Empty) != Answer4)
            {
                return false;
            }

            if ((delegateUser.Answer5 ?? string.Empty) != Answer5)
            {
                return false;
            }

            if ((delegateUser.Answer6 ?? string.Empty) != Answer6)
            {
                return false;
            }

            if ((delegateUser.EmailAddress ?? string.Empty) != Email)
            {
                return false;
            }

            if (delegateUser.ProfessionalRegistrationNumber != Prn)
            {
                return false;
            }

            return DelegateDownloadFileService.GetHasPrnForDelegate(
                delegateUser.HasBeenPromptedForPrn,
                delegateUser.ProfessionalRegistrationNumber
            ) == HasPrn;
        }
    }
}
