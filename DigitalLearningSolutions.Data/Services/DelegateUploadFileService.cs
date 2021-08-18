﻿namespace DigitalLearningSolutions.Data.Services
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
        private readonly IUserService userService;

        public DelegateUploadFileService(
            IJobGroupsDataService jobGroupsDataService,
            IUserDataService userDataService,
            IRegistrationDataService registrationDataService,
            IUserService userService
        )
        {
            this.userDataService = userDataService;
            this.registrationDataService = registrationDataService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.userService = userService;
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
                if (!delegateRow.IsValid(jobGroupIds))
                {
                    continue;
                }

                var delegateUserByCandidateNumber = !string.IsNullOrEmpty(delegateRow.CandidateNumber)
                    ? userDataService.GetDelegateUserByCandidateNumber(delegateRow.CandidateNumber, centreId)
                    : null;

                if (!string.IsNullOrEmpty(delegateRow.CandidateNumber) && delegateUserByCandidateNumber == null)
                {
                    delegateRow.Error = BulkUploadResult.ErrorReason.NoRecordForDelegateId;
                    continue;
                }

                var delegateUserByAliasId = !string.IsNullOrEmpty(delegateRow.AliasId)
                    ? userDataService.GetDelegateUserByAliasId(delegateRow.AliasId, centreId)
                    : null;

                if (delegateUserByAliasId != null && delegateUserByCandidateNumber != null &&
                    delegateUserByAliasId.CandidateNumber != delegateUserByCandidateNumber.CandidateNumber)
                {
                    delegateRow.Error = BulkUploadResult.ErrorReason.AliasIdInUse;
                    continue;
                }

                if (delegateUserByCandidateNumber != null)
                {
                    if (delegateRow.Email != delegateUserByCandidateNumber.EmailAddress &&
                        !userService.IsEmailValidForCentre(delegateRow.Email!, centreId))
                    {
                        delegateRow.Error = BulkUploadResult.ErrorReason.EmailAddressInUse;
                        continue;
                    }

                    if (!RecordNeedsUpdating(delegateUserByCandidateNumber, delegateRow))
                    {
                        delegateRow.RowStatus = RowStatus.Skipped;
                        continue;
                    }

                    userDataService.UpdateDelegate(
                        delegateUserByCandidateNumber.Id,
                        delegateRow.FirstName!,
                        delegateRow.LastName!,
                        delegateRow.JobGroupId!.Value,
                        delegateRow.Active!.Value,
                        delegateRow.Answer1,
                        delegateRow.Answer2,
                        delegateRow.Answer3,
                        delegateRow.Answer4,
                        delegateRow.Answer5,
                        delegateRow.Answer6,
                        delegateRow.AliasId,
                        delegateRow.Email!
                    );
                    delegateRow.RowStatus = RowStatus.Updated;
                    continue;
                }

                if (delegateUserByAliasId != null)
                {
                    if (delegateRow.Email != delegateUserByAliasId.EmailAddress &&
                        !userService.IsEmailValidForCentre(delegateRow.Email!, centreId))
                    {
                        delegateRow.Error = BulkUploadResult.ErrorReason.EmailAddressInUse;
                        continue;
                    }

                    if (!RecordNeedsUpdating(delegateUserByAliasId, delegateRow))
                    {
                        delegateRow.RowStatus = RowStatus.Skipped;
                        continue;
                    }

                    userDataService.UpdateDelegate(
                        delegateUserByAliasId.Id,
                        delegateRow.FirstName!,
                        delegateRow.LastName!,
                        delegateRow.JobGroupId!.Value,
                        delegateRow.Active!.Value,
                        delegateRow.Answer1,
                        delegateRow.Answer2,
                        delegateRow.Answer3,
                        delegateRow.Answer4,
                        delegateRow.Answer5,
                        delegateRow.Answer6,
                        delegateRow.AliasId,
                        delegateRow.Email!
                    );
                    delegateRow.RowStatus = RowStatus.Updated;
                    continue;
                }

                if (delegateUserByAliasId == null && delegateUserByCandidateNumber == null)
                {
                    if (!userService.IsEmailValidForCentre(delegateRow.Email!, centreId))
                    {
                        delegateRow.Error = BulkUploadResult.ErrorReason.EmailAddressInUse;
                        continue;
                    }

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

        private bool RecordNeedsUpdating(DelegateUser delegateUser, DelegateTableRow delegateRow)
        {
            if (delegateRow.CandidateNumber != null && (delegateUser.AliasId ?? string.Empty) != delegateRow.AliasId)
            {
                return true;
            }

            if ((delegateUser.FirstName ?? string.Empty) != delegateRow.FirstName)
            {
                return true;
            }

            if (delegateUser.LastName != delegateRow.LastName)
            {
                return true;
            }

            if (delegateUser.JobGroupId != delegateRow.JobGroupId!.Value)
            {
                return true;
            }

            if (delegateUser.Active != delegateRow.Active!.Value)
            {
                return true;
            }

            if ((delegateUser.Answer1 ?? string.Empty) != delegateRow.Answer1)
            {
                return true;
            }

            if ((delegateUser.Answer2 ?? string.Empty) != delegateRow.Answer2)
            {
                return true;
            }

            if ((delegateUser.Answer3 ?? string.Empty) != delegateRow.Answer3)
            {
                return true;
            }

            if ((delegateUser.Answer4 ?? string.Empty) != delegateRow.Answer4)
            {
                return true;
            }

            if ((delegateUser.Answer5 ?? string.Empty) != delegateRow.Answer5)
            {
                return true;
            }

            if ((delegateUser.Answer6 ?? string.Empty) != delegateRow.Answer6)
            {
                return true;
            }

            if ((delegateUser.EmailAddress ?? string.Empty) != delegateRow.Email)
            {
                return true;
            }

            return false;
        }

        private void RegisterDelegate(DelegateTableRow delegateRow, DateTime? welcomeEmailDate, int centreId)
        {
            var model = new DelegateRegistrationModel(delegateRow, centreId, welcomeEmailDate);
            var status = registrationDataService.RegisterDelegateByCentre(model);
            switch (status)
            {
                case "-1":
                case "-2":
                case "-3":
                case "-4":
                    throw new ArgumentOutOfRangeException(
                        nameof(status),
                        status,
                        "Unknown return value when creating delegate record."
                    );
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
    }
}
