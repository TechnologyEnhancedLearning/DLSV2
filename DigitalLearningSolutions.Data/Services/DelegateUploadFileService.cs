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
        public BulkUploadResult ProcessDelegatesFile(IXLTable table, int centreId);
        public IXLTable OpenDelegatesTable(IFormFile file);
    }

    public class DelegateUploadFileService : IDelegateUploadFileService
    {
        private const string DelegatesSheetName = "DelegatesBulkUpload";
        private readonly IJobGroupsDataService jobGroupsDataService;

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
            this.jobGroupsDataService = jobGroupsDataService;
        }

        public BulkUploadResult ProcessDelegatesFile(IXLTable table, int centreId)
        {
            if (!ValidateHeaders(table))
            {
                throw new InvalidHeadersException();
            }

            var jobGroupIds = jobGroupsDataService.GetJobGroupsAlphabetical().Select(item => item.id).ToList();
            var (registered, updated, skipped) = (0, 0, 0);
            var errors = new List<(int, BulkUploadResult.ErrorReasons)>();
            var delegateRows = table.Rows().Skip(1).Select(row => new DelegateTableRow(table, row));

            foreach (var delegateRow in delegateRows)
            {
                var errorReason = delegateRow.ValidateFields(jobGroupIds);
                if (errorReason.HasValue)
                {
                    errors.Add((delegateRow.RowNumber, errorReason.Value));
                    continue;
                }

                bool? approved;
                try
                {
                    approved = TryGetExistingApprovedStatus(delegateRow.DelegateId, delegateRow.AliasId, centreId);
                }
                catch (UserAccountNotFoundException)
                {
                    errors.Add((delegateRow.RowNumber, BulkUploadResult.ErrorReasons.NoRecordForDelegateId));
                    continue;
                }

                if (approved.HasValue)
                {
                    var record = MapRowToDelegateRecord(delegateRow, centreId, approved.Value);
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
                            errors.Add((delegateRow.RowNumber, BulkUploadResult.ErrorReasons.UnexpectedErrorForUpdate));
                            break;
                        case -2:
                            errors.Add((delegateRow.RowNumber, BulkUploadResult.ErrorReasons.ParameterError));
                            break;
                        case -3:
                            errors.Add((delegateRow.RowNumber, BulkUploadResult.ErrorReasons.AliasIdInUse));
                            break;
                        case -5:
                            errors.Add((delegateRow.RowNumber, BulkUploadResult.ErrorReasons.EmailAddressInUse));
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
                    var model = MapRowToDelegateRegistrationModel(delegateRow, centreId);
                    var status = registrationDataService.RegisterDelegateByCentre(model);
                    switch (status)
                    {
                        case "-1":
                            errors.Add((delegateRow.RowNumber, BulkUploadResult.ErrorReasons.UnexpectedErrorForCreate));
                            break;
                        case "-2":
                            errors.Add((delegateRow.RowNumber, BulkUploadResult.ErrorReasons.ParameterError));
                            break;
                        case "-3":
                            errors.Add((delegateRow.RowNumber, BulkUploadResult.ErrorReasons.AliasIdInUse));
                            break;
                        case "-4":
                            errors.Add((delegateRow.RowNumber, BulkUploadResult.ErrorReasons.EmailAddressInUse));
                            break;
                        default:
                            registered += 1;
                            break;
                    }
                }
            }

            return new BulkUploadResult(table.RowCount() - 1, registered, updated, skipped, errors);
        }

        public IXLTable OpenDelegatesTable(IFormFile file)
        {
            var workbook = new XLWorkbook(file.OpenReadStream());
            var worksheet = workbook.Worksheet(DelegatesSheetName);
            var table = worksheet.Tables.Table(0);
            return table;
        }

        private static DelegateRecord MapRowToDelegateRecord(
            DelegateTableRow row,
            int centreId,
            bool approved
        )
        {
            return new DelegateRecord(
                centreId,
                row.DelegateId,
                row.FirstName,
                row.LastName!,
                int.Parse(row.JobGroupId!),
                bool.Parse(row.Active!),
                row.Answer1,
                row.Answer2,
                row.Answer3,
                row.Answer4,
                row.Answer5,
                row.Answer6,
                row.AliasId,
                approved,
                row.Email
            );
        }

        private static DelegateRegistrationModel MapRowToDelegateRegistrationModel(
            DelegateTableRow row,
            int centreId
        )
        {
            return new DelegateRegistrationModel(
                row.FirstName!,
                row.LastName!,
                row.Email,
                centreId,
                int.Parse(row.JobGroupId!),
                null,
                row.Answer1,
                row.Answer2,
                row.Answer3,
                row.Answer4,
                row.Answer5,
                row.Answer6,
                row.AliasId
            );
        }

        private bool ValidateHeaders(IXLTable table)
        {
            var expectedHeaders = new List<string>
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
            }.OrderBy(x => x);
            var actualHeaders = table.Fields.Select(x => x.Name).OrderBy(x => x);
            return actualHeaders.SequenceEqual(expectedHeaders);
        }

        private bool? TryGetExistingApprovedStatus(string? delegateId, string? aliasId, int centreId)
        {
            if (!string.IsNullOrWhiteSpace(delegateId))
            {
                var approvedStatus = userDataService.GetApprovedStatusFromCandidateNumber(delegateId, centreId);
                if (!approvedStatus.HasValue)
                {
                    throw new UserAccountNotFoundException(string.Empty);
                }

                return approvedStatus.Value;
            }

            return !string.IsNullOrWhiteSpace(aliasId)
                ? userDataService.GetApprovedStatusFromAliasId(aliasId, centreId)
                : null;
        }

        public class DelegateTableRow
        {
            public DelegateTableRow(IXLTable table, IXLRangeRow row)
            {
                string? FindFieldValue(string name)
                {
                    var col = table.FindColumn(col => col.FirstCell().Value.ToString() == name).ColumnNumber();
                    return row.Cell(col).GetValue<string?>();
                }

                RowNumber = row.RowNumber();
                DelegateId = FindFieldValue("DelegateID");
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
            public string? DelegateId { get; set; }
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
}
