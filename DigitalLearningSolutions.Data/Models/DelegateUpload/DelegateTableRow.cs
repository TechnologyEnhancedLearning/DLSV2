namespace DigitalLearningSolutions.Data.Models.DelegateUpload
{
    using System.Collections.Generic;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.Models.User;

    public enum RowStatus
    {
        NotYetProcessed,
        Skipped,
        Registered,
        Updated
    }

    public class DelegateTableRow
    {
        public DelegateTableRow(IXLTable table, IXLRangeRow row)
        {
            string? FindFieldValue(string name)
            {
                var colNumber = table.FindColumn(col => col.FirstCell().Value.ToString() == name).ColumnNumber();
                return row.Cell(colNumber).GetValue<string?>();
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
            Email = FindFieldValue("EmailAddress");
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

        public BulkUploadResult.ErrorReason? Error { get; set; }
        public RowStatus RowStatus { get; set; }

        public bool Validate(IEnumerable<int> allowedJobGroupIds)
        {
            if (!JobGroupId.HasValue || !allowedJobGroupIds.Contains(JobGroupId.Value))
            {
                Error = BulkUploadResult.ErrorReason.InvalidJobGroupId;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                Error = BulkUploadResult.ErrorReason.InvalidLastName;
            }

            if (string.IsNullOrEmpty(FirstName))
            {
                Error = BulkUploadResult.ErrorReason.InvalidFirstName;
            }

            if (string.IsNullOrEmpty(Email))
            {
                Error = BulkUploadResult.ErrorReason.InvalidEmail;
            }

            if (!Active.HasValue)
            {
                Error = BulkUploadResult.ErrorReason.InvalidActive;
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

            return true;
        }
    }
}
