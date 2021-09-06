﻿using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DigitalLearningSolutions.Data.Tests")]

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

        internal IXLTable OpenDelegatesTable(IFormFile file)
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

        internal BulkUploadResult ProcessDelegatesTable(IXLTable table, int centreId, DateTime? welcomeEmailDate = null)
        {
            var jobGroupIds = jobGroupsDataService.GetJobGroupsAlphabetical().Select(item => item.id).ToList();
            var delegateRows = table.Rows().Skip(1).Select(row => new DelegateTableRow(table, row)).ToList();

            foreach (var delegateRow in delegateRows)
            {
                ProcessDelegateRow(centreId, welcomeEmailDate, delegateRow, jobGroupIds);
            }

            return new BulkUploadResult(delegateRows);
        }

        private void ProcessDelegateRow(
            int centreId,
            DateTime? welcomeEmailDate,
            DelegateTableRow delegateRow,
            IEnumerable<int> jobGroupIds
        )
        {
            if (!delegateRow.Validate(jobGroupIds))
            {
                return;
            }

            var delegateUserByCandidateNumber =
                GetDelegateUserByCandidateNumberOrDefault(centreId, delegateRow.CandidateNumber);

            if (!string.IsNullOrEmpty(delegateRow.CandidateNumber) && delegateUserByCandidateNumber == null)
            {
                delegateRow.Error = BulkUploadResult.ErrorReason.NoRecordForDelegateId;
                return;
            }

            var delegateUserByAliasId = GetDelegateUserByAliasIdOrDefault(centreId, delegateRow.AliasId);

            if (delegateUserByAliasId != null && delegateUserByCandidateNumber != null &&
                delegateUserByAliasId.CandidateNumber != delegateUserByCandidateNumber.CandidateNumber)
            {
                delegateRow.Error = BulkUploadResult.ErrorReason.AliasIdInUse;
                return;
            }

            var userToUpdate = delegateUserByCandidateNumber ?? delegateUserByAliasId;
            if (userToUpdate == null)
            {
                if (!userService.IsDelegateEmailValidForCentre(delegateRow.Email!, centreId))
                {
                    delegateRow.Error = BulkUploadResult.ErrorReason.EmailAddressInUse;
                    return;
                }

                RegisterDelegate(delegateRow, welcomeEmailDate, centreId);
            }
            else
            {
                ProcessPotentialUpdate(centreId, delegateRow, userToUpdate);
            }
        }

        private DelegateUser? GetDelegateUserByCandidateNumberOrDefault(int centreId, string? candidateNumber)
        {
            return !string.IsNullOrEmpty(candidateNumber)
                ? userDataService.GetDelegateUserByCandidateNumber(candidateNumber, centreId)
                : null;
        }

        private DelegateUser? GetDelegateUserByAliasIdOrDefault(int centreId, string? aliasId)
        {
            return !string.IsNullOrEmpty(aliasId)
                ? userDataService.GetDelegateUserByAliasId(aliasId, centreId)
                : null;
        }

        private void ProcessPotentialUpdate(int centreId, DelegateTableRow delegateRow, DelegateUser delegateUser)
        {
            if (delegateRow.Email != delegateUser.EmailAddress &&
                !userService.IsDelegateEmailValidForCentre(delegateRow.Email!, centreId))
            {
                delegateRow.Error = BulkUploadResult.ErrorReason.EmailAddressInUse;
                return;
            }

            if (delegateRow.MatchesDelegateUser(delegateUser))
            {
                delegateRow.RowStatus = RowStatus.Skipped;
                return;
            }

            UpdateDelegate(delegateRow, delegateUser);
        }

        private void UpdateDelegate(DelegateTableRow delegateRow, DelegateUser delegateUser)
        {
            try
            {
                userDataService.UpdateDelegate(
                    delegateUser.Id,
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
            }
            catch
            {
                delegateRow.Error = BulkUploadResult.ErrorReason.UnexpectedErrorForUpdate;
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
