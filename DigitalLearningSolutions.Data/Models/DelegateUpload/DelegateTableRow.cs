namespace DigitalLearningSolutions.Data.Models.DelegateUpload
{
    using System.Collections.Generic;
    using System.Linq;
    using ClosedXML.Excel;

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
            JobGroupId = FindFieldValue("JobGroupID");
            Active = FindFieldValue("Active");
            Answer1 = FindFieldValue("Answer1");
            Answer2 = FindFieldValue("Answer2");
            Answer3 = FindFieldValue("Answer3");
            Answer4 = FindFieldValue("Answer4");
            Answer5 = FindFieldValue("Answer5");
            Answer6 = FindFieldValue("Answer6");
            AliasId = FindFieldValue("AliasID");
            Email = FindFieldValue("EmailAddress");
        }

        public int RowNumber { get; set; }
        public string? CandidateNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? JobGroupId { get; set; }
        public string? Active { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Answer6 { get; set; }
        public string? AliasId { get; set; }
        public string? Email { get; set; }

        public BulkUploadResult.ErrorReasons? ValidateFields(IEnumerable<int> allowedJobGroupIds)
        {
            if (!int.TryParse(JobGroupId, out var jobGroupId) || !allowedJobGroupIds.Contains(jobGroupId))
            {
                return BulkUploadResult.ErrorReasons.InvalidJobGroupId;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                return BulkUploadResult.ErrorReasons.InvalidLastName;
            }

            if (string.IsNullOrEmpty(FirstName))
            {
                return BulkUploadResult.ErrorReasons.InvalidFirstName;
            }

            if (!bool.TryParse(Active, out _))
            {
                return BulkUploadResult.ErrorReasons.InvalidActive;
            }

            return null;
        }
    }
}
