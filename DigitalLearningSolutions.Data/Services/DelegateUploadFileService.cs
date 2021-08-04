namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using Microsoft.AspNetCore.Http;

    public class BulkUploadResult
    {
        public enum ErrorReasons
        {
            InvalidJobGroupId,
            InvalidLastName,
            InvalidFirstName,
            InvalidActive,
            NoRecordForDelegateId
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

    public interface IDelegateUploadFileService
    {
        public BulkUploadResult ProcessDelegatesFile(IFormFile file, int centreId);
    }

    public class DelegateUploadFileService : IDelegateUploadFileService
    {
        private const string DelegatesSheetName = "DelegatesBulkUpload";

        private readonly List<string> headers = new List<string>
        {
            "LastName",
            "FirstName",
            "DelegateID",
            "AliasID",
            "JobGroupID",
            "Answer1",
            "Answer2",
            "Answer3",
            "Answer4",
            "Answer5",
            "Answer6",
            "Active",
            "EmailAddress"
        };

        private readonly IEnumerable<int> jobGroupIds;
        private readonly IUserDataService userDataService;

        public DelegateUploadFileService(IJobGroupsDataService jobGroupsDataService, IUserDataService userDataService)
        {
            this.userDataService = userDataService;
            jobGroupIds = jobGroupsDataService.GetJobGroupsAlphabetical()
                .Select(item => item.id);
        }

        public BulkUploadResult ProcessDelegatesFile(IFormFile file, int centreId)
        {
            var table = OpenDelegatesTable(file);
            if (!ValidateHeaders(table))
            {
                throw new InvalidHeadersException();
            }

            var (registered, updated, skipped) = (0, 0, 0);
            var errors = new List<(int, BulkUploadResult.ErrorReasons)>();

            foreach (var row in table.Rows().Skip(1))
            {
                var errorReason = ValidateFields(table, row);
                if (errorReason.HasValue)
                {
                    errors.Add((row.RowNumber(), errorReason.Value));
                    continue;
                }

                var delegateIdCol = FindColumn(table, "DelegateID");
                var aliasIdCol = FindColumn(table, "AliasID");
                bool approved;
                if (row.Cell(delegateIdCol).TryGetValue<string>(out var delegateId))
                {
                    var approvedStatus = userDataService.GetApprovedStatusFromCandidateNumber(delegateId, centreId);
                    if (!approvedStatus.HasValue)
                    {
                        errors.Add((row.RowNumber(), BulkUploadResult.ErrorReasons.NoRecordForDelegateId));
                        continue;
                    }

                    approved = approvedStatus.Value;
                }
                else if (row.Cell(aliasIdCol).TryGetValue<string>(out var aliasId))
                {
                    approved = userDataService.GetApprovedStatusFromAliasId(aliasId, centreId) ?? true;
                }
            }

            return new BulkUploadResult(table.RowCount(), registered, updated, skipped, errors);
        }

        private BulkUploadResult.ErrorReasons? ValidateFields(IXLTable table, IXLRangeRow row)
        {
            var jobGroupCol = FindColumn(table, "JobGroupID");
            var lastNameCol = FindColumn(table, "LastName");
            var firstNameCol = FindColumn(table, "FirstName");
            var activeCol = FindColumn(table, "Active");

            if (!row.Cell(jobGroupCol).TryGetValue<int>(out var jobGroupId) || !jobGroupIds.Contains(jobGroupId))
            {
                return BulkUploadResult.ErrorReasons.InvalidJobGroupId;
            }

            if (!row.Cell(lastNameCol).TryGetValue<string>(out var lastName) || string.IsNullOrEmpty(lastName))
            {
                return BulkUploadResult.ErrorReasons.InvalidLastName;
            }

            if (!row.Cell(firstNameCol).TryGetValue<string>(out var firstName) || string.IsNullOrEmpty(firstName))
            {
                return BulkUploadResult.ErrorReasons.InvalidFirstName;
            }

            if (!row.Cell(activeCol).TryGetValue<bool>(out _))
            {
                return BulkUploadResult.ErrorReasons.InvalidActive;
            }

            return null;
        }

        private int FindColumn(IXLTable table, string name)
        {
            return table.FindColumn(col => col.FirstCell().Value.ToString() == name).ColumnNumber();
        }

        private IXLTable OpenDelegatesTable(IFormFile file)
        {
            var workbook = new XLWorkbook(file.OpenReadStream());
            var worksheet = workbook.Worksheet(DelegatesSheetName);
            var table = worksheet.Tables.Table(0);
            return table;
        }

        private bool ValidateHeaders(IXLTable table)
        {
            var actualHeaders = table.Fields.Select(x => x.Name).OrderBy(x => x);
            var expectedHeaders = headers.OrderBy(x => x);
            return actualHeaders.SequenceEqual(expectedHeaders);
        }
    }
}
