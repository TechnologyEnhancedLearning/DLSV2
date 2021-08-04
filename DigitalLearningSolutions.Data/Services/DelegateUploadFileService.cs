namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.User;
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

                var approved = GetApprovedStatusOrDefault(table, row, centreId);
                if (!approved.HasValue)
                {
                    errors.Add((row.RowNumber(), BulkUploadResult.ErrorReasons.NoRecordForDelegateId));
                }
            }

            return new BulkUploadResult(table.RowCount(), registered, updated, skipped, errors);
        }

        private static IXLTable OpenDelegatesTable(IFormFile file)
        {
            var workbook = new XLWorkbook(file.OpenReadStream());
            var worksheet = workbook.Worksheet(DelegatesSheetName);
            var table = worksheet.Tables.Table(0);
            return table;
        }

        private static int FindColumn(IXLTable table, string name)
        {
            return table.FindColumn(col => col.FirstCell().Value.ToString() == name).ColumnNumber();
        }

        private static DelegateRecord MapRowToRecord(IXLTable table, IXLRangeRow row, int centreId, bool approved)
        {
            var lastName = row.Cell(FindColumn(table, "LastName")).GetValue<string>();
            var firstName = row.Cell(FindColumn(table, "FirstName")).GetValue<string>();
            var delegateId = row.Cell(FindColumn(table, "DelegateID")).GetValue<string>();
            var aliasId = row.Cell(FindColumn(table, "AliasID")).GetValue<string?>();
            var jobGroupId = row.Cell(FindColumn(table, "JobGroupID")).GetValue<int>();
            var active = row.Cell(FindColumn(table, "Active")).GetValue<bool>();
            var emailAddress = row.Cell(FindColumn(table, "EmailAddress")).GetValue<string?>();
            var answer1 = row.Cell(FindColumn(table, "Answer1")).GetValue<string?>();
            var answer2 = row.Cell(FindColumn(table, "Answer2")).GetValue<string?>();
            var answer3 = row.Cell(FindColumn(table, "Answer3")).GetValue<string?>();
            var answer4 = row.Cell(FindColumn(table, "Answer4")).GetValue<string?>();
            var answer5 = row.Cell(FindColumn(table, "Answer5")).GetValue<string?>();
            var answer6 = row.Cell(FindColumn(table, "Answer6")).GetValue<string?>();
            return new DelegateRecord(
                centreId,
                delegateId,
                firstName,
                lastName,
                jobGroupId,
                active,
                answer1,
                answer2,
                answer3,
                answer4,
                answer5,
                answer6,
                aliasId,
                approved,
                emailAddress
            );
        }

        private bool ValidateHeaders(IXLTable table)
        {
            var actualHeaders = table.Fields.Select(x => x.Name).OrderBy(x => x);
            var expectedHeaders = headers.OrderBy(x => x);
            return actualHeaders.SequenceEqual(expectedHeaders);
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

        private bool? GetApprovedStatusOrDefault(IXLTable table, IXLRangeRow row, int centreId)
        {
            var delegateIdCol = FindColumn(table, "DelegateID");
            var aliasIdCol = FindColumn(table, "AliasID");

            if (row.Cell(delegateIdCol).TryGetValue<string>(out var delegateId))
            {
                var approvedStatus = userDataService.GetApprovedStatusFromCandidateNumber(delegateId, centreId);
                return approvedStatus;
            }

            if (row.Cell(aliasIdCol).TryGetValue<string>(out var aliasId))
            {
                return userDataService.GetApprovedStatusFromAliasId(aliasId, centreId) ?? true;
            }

            return true;
        }
    }
}
