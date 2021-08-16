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
            var delegateRows = table.Rows().Skip(1).Select(row => new DelegateTableRow(table, row)).ToList();

            foreach (var delegateRow in delegateRows)
            {
                if (IsDelegateRowInvalid(centreId, delegateRow, jobGroupIds, out var approved))
                {
                    continue;
                }

                if (approved.HasValue)
                {
                    UpdateDelegate(delegateRow, approved.Value, centreId);
                }
                else
                {
                    RegisterDelegate(delegateRow, welcomeEmailDate, centreId);
                }
            }

            return new BulkUploadResult(delegateRows);
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

        private bool IsDelegateRowInvalid(int centreId, DelegateTableRow delegateRow, IEnumerable<int> jobGroupIds, out bool? approved)
        {
            approved = null;
            return !delegateRow.IsValid(jobGroupIds) || !TryGetExistingApprovedStatus(delegateRow, centreId, out approved);
        }

        private void UpdateDelegate(DelegateTableRow delegateRow, bool approved, int centreId)
        {
            var record = new DelegateRecord(delegateRow, centreId, approved);
            var status = userDataService.UpdateDelegateRecord(record);
            switch (status)
            {
                case 0:
                    delegateRow.RowStatus = RowStatus.Updated;
                    break;
                case 1:
                    delegateRow.RowStatus = RowStatus.Skipped;
                    break;
                case 2:
                    delegateRow.RowStatus = RowStatus.Registered;
                    break;
                case -1:
                case -4:
                    delegateRow.Error = BulkUploadResult.ErrorReason.UnexpectedErrorForUpdate;
                    break;
                case -2:
                    delegateRow.Error = BulkUploadResult.ErrorReason.ParameterError;
                    break;
                case -3:
                    delegateRow.Error = BulkUploadResult.ErrorReason.AliasIdInUse;
                    break;
                case -5:
                    delegateRow.Error = BulkUploadResult.ErrorReason.EmailAddressInUse;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(status),
                        status,
                        "Unknown return value when updating delegate record."
                    );
            }
        }

        private void RegisterDelegate(DelegateTableRow delegateRow, DateTime? welcomeEmailDate, int centreId)
        {
            var model = new DelegateRegistrationModel(delegateRow, centreId, welcomeEmailDate);
            var status = registrationDataService.RegisterDelegateByCentre(model);
            switch (status)
            {
                case "-1":
                    delegateRow.Error = BulkUploadResult.ErrorReason.UnexpectedErrorForCreate;
                    break;
                case "-2":
                    delegateRow.Error = BulkUploadResult.ErrorReason.ParameterError;
                    break;
                case "-3":
                    delegateRow.Error = BulkUploadResult.ErrorReason.AliasIdInUse;
                    break;
                case "-4":
                    delegateRow.Error = BulkUploadResult.ErrorReason.EmailAddressInUse;
                    break;
                default:
                    delegateRow.RowStatus = RowStatus.Registered;
                    break;
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

        private bool TryGetExistingApprovedStatus(DelegateTableRow delegateRow, int centreId, out bool? approved)
        {
            if (!string.IsNullOrWhiteSpace(delegateRow.CandidateNumber))
            {
                approved = userDataService.GetApprovedStatusFromCandidateNumber(delegateRow.CandidateNumber, centreId);
                if (approved.HasValue)
                {
                    return true;
                }

                delegateRow.Error = BulkUploadResult.ErrorReason.NoRecordForDelegateId;
                return false;
            }

            approved = !string.IsNullOrWhiteSpace(delegateRow.AliasId)
                ? userDataService.GetApprovedStatusFromAliasId(delegateRow.AliasId, centreId)
                : null;
            return true;
        }
    }
}
