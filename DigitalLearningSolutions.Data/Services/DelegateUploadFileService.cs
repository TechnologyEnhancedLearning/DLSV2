namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Register;
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
            NoRecordForDelegateId,
            UnexpectedErrorForUpdate,
            UnexpectedErrorForCreate,
            ParameterError,
            AliasIdInUse,
            EmailAddressInUse
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
        private readonly IRegistrationDataService registrationDataService;
        private readonly IUserDataService userDataService;

        public DelegateUploadFileService(
            IJobGroupsDataService jobGroupsDataService,
            IUserDataService userDataService,
            IRegistrationDataService registrationDataService
        )
        {
            this.userDataService = userDataService;
            this.registrationDataService = registrationDataService;
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

                bool? approved;
                try
                {
                    approved = GetApprovedStatusForUpdate(table, row, centreId);
                }
                catch (UserAccountNotFoundException)
                {
                    errors.Add((row.RowNumber(), BulkUploadResult.ErrorReasons.NoRecordForDelegateId));
                    continue;
                }

                if (approved.HasValue)
                {
                    var record = MapRowToDelegateRecord(table, row, centreId, approved.Value);
                    var status = userDataService.UpdateDelegateRecord(record);
                    switch (status)
                    {
                        case 0:
                            updated += 1;
                            break;
                        case 1:
                            skipped += 1;
                            break;
                        case 2:
                            registered += 1;
                            break;
                        case -1:
                        case -4:
                            errors.Add((row.RowNumber(), BulkUploadResult.ErrorReasons.UnexpectedErrorForUpdate));
                            break;
                        case -2:
                            errors.Add((row.RowNumber(), BulkUploadResult.ErrorReasons.ParameterError));
                            break;
                        case -3:
                            errors.Add((row.RowNumber(), BulkUploadResult.ErrorReasons.AliasIdInUse));
                            break;
                        case -5:
                            errors.Add((row.RowNumber(), BulkUploadResult.ErrorReasons.EmailAddressInUse));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(status),
                                status,
                                "Unknown return value when updating delegate record."
                            );
                    }
                }
                else
                {
                    var model = MapRowToDelegateRegistrationModel(table, row, centreId);
                    var status = registrationDataService.RegisterDelegateByCentre(model);
                    switch (status)
                    {
                        case "-1":
                            errors.Add((row.RowNumber(), BulkUploadResult.ErrorReasons.UnexpectedErrorForCreate));
                            break;
                        case "-2":
                            errors.Add((row.RowNumber(), BulkUploadResult.ErrorReasons.ParameterError));
                            break;
                        case "-3":
                            errors.Add((row.RowNumber(), BulkUploadResult.ErrorReasons.AliasIdInUse));
                            break;
                        case "-4":
                            errors.Add((row.RowNumber(), BulkUploadResult.ErrorReasons.EmailAddressInUse));
                            break;
                        default:
                            registered += 1;
                            break;
                    }
                }
            }

            return new BulkUploadResult(table.RowCount() - 1, registered, updated, skipped, errors);
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

        private static DelegateRecord MapRowToDelegateRecord(
            IXLTable table,
            IXLRangeRow row,
            int centreId,
            bool approved
        )
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

        private static DelegateRegistrationModel MapRowToDelegateRegistrationModel(
            IXLTable table,
            IXLRangeRow row,
            int centreId
        )
        {
            var lastName = row.Cell(FindColumn(table, "LastName")).GetValue<string>();
            var firstName = row.Cell(FindColumn(table, "FirstName")).GetValue<string>();
            var aliasId = row.Cell(FindColumn(table, "AliasID")).GetValue<string?>();
            var jobGroupId = row.Cell(FindColumn(table, "JobGroupID")).GetValue<int>();
            var emailAddress = row.Cell(FindColumn(table, "EmailAddress")).GetValue<string?>();
            var answer1 = row.Cell(FindColumn(table, "Answer1")).GetValue<string?>();
            var answer2 = row.Cell(FindColumn(table, "Answer2")).GetValue<string?>();
            var answer3 = row.Cell(FindColumn(table, "Answer3")).GetValue<string?>();
            var answer4 = row.Cell(FindColumn(table, "Answer4")).GetValue<string?>();
            var answer5 = row.Cell(FindColumn(table, "Answer5")).GetValue<string?>();
            var answer6 = row.Cell(FindColumn(table, "Answer6")).GetValue<string?>();
            return new DelegateRegistrationModel(
                firstName,
                lastName,
                emailAddress,
                centreId,
                jobGroupId,
                null,
                answer1,
                answer2,
                answer3,
                answer4,
                answer5,
                answer6,
                aliasId
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

        private bool? GetApprovedStatusForUpdate(IXLTable table, IXLRangeRow row, int centreId)
        {
            var delegateIdCol = FindColumn(table, "DelegateID");
            var aliasIdCol = FindColumn(table, "AliasID");

            if (row.Cell(delegateIdCol).TryGetValue<string>(out var delegateId) &&
                !string.IsNullOrWhiteSpace(delegateId))
            {
                var approvedStatus = userDataService.GetApprovedStatusFromCandidateNumber(delegateId, centreId);
                if (!approvedStatus.HasValue)
                {
                    throw new UserAccountNotFoundException(string.Empty);
                }

                return approvedStatus.Value;
            }

            if (row.Cell(aliasIdCol).TryGetValue<string>(out var aliasId) && !string.IsNullOrWhiteSpace(delegateId))
            {
                return userDataService.GetApprovedStatusFromAliasId(aliasId, centreId);
            }

            return null;
        }
    }
}
