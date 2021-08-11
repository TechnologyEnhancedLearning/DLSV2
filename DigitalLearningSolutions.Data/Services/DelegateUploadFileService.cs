namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.AspNetCore.Http;

    public interface IDelegateUploadFileService
    {
        public BulkUploadResult ProcessDelegatesFile(IFormFile file, int centreId, DateTime? welcomeEmailDate = null);
        public BulkUploadResult ProcessDelegatesTable(IXLTable table, int centreId, DateTime? welcomeEmailDate = null);
        public IXLTable OpenDelegatesTable(IFormFile file);
    }

    public class DelegateUploadFileService : IDelegateUploadFileService
    {
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

        public BulkUploadResult ProcessDelegatesFile(IFormFile file, int centreId, DateTime? welcomeEmailDate)
        {
            var table = OpenDelegatesTable(file);
            return ProcessDelegatesTable(table, centreId, welcomeEmailDate);
        }

        public BulkUploadResult ProcessDelegatesTable(IXLTable table, int centreId, DateTime? welcomeEmailDate)
        {
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
                    approved = TryGetExistingApprovedStatus(delegateRow.CandidateNumber, delegateRow.AliasId, centreId);
                }
                catch (UserAccountNotFoundException)
                {
                    errors.Add((delegateRow.RowNumber, BulkUploadResult.ErrorReasons.NoRecordForDelegateId));
                    continue;
                }

                if (approved.HasValue)
                {
                    UpdateDelegate(
                        delegateRow,
                        approved.Value,
                        centreId,
                        ref updated,
                        ref skipped,
                        ref registered,
                        errors
                    );
                }
                else
                {
                    RegisterDelegate(delegateRow, welcomeEmailDate, centreId, ref registered, errors);
                }
            }

            return new BulkUploadResult(table.RowCount() - 1, registered, updated, skipped, errors);
        }

        public IXLTable OpenDelegatesTable(IFormFile file)
        {
            var workbook = new XLWorkbook(file.OpenReadStream());
            var worksheet = workbook.Worksheet(DelegateDownloadFileService.DelegatesSheetName);
            var table = worksheet.Tables.Table(0);

            if (!ValidateHeaders(table))
            {
                throw new InvalidHeadersException();
            }

            return table;
        }

        private void RegisterDelegate(
            DelegateTableRow delegateRow,
            DateTime? welcomeEmailDate,
            int centreId,
            ref int registered,
            List<(int, BulkUploadResult.ErrorReasons)> errors
        )
        {
            var model = new DelegateRegistrationModel(delegateRow, centreId, welcomeEmailDate);
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

        private void UpdateDelegate(
            DelegateTableRow delegateRow,
            bool approved,
            int centreId,
            ref int updated,
            ref int skipped,
            ref int registered,
            List<(int, BulkUploadResult.ErrorReasons)> errors
        )
        {
            var record = new DelegateRecord(delegateRow, centreId, approved);
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

        private static bool ValidateHeaders(IXLTable table)
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

        private bool? TryGetExistingApprovedStatus(string? candidateNumber, string? aliasId, int centreId)
        {
            if (!string.IsNullOrWhiteSpace(candidateNumber))
            {
                var approvedStatus = userDataService.GetApprovedStatusFromCandidateNumber(candidateNumber, centreId);
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
    }
}
