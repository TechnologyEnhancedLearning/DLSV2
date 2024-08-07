﻿using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.DataServices.UserDataService;
using DigitalLearningSolutions.Data.Exceptions;
using DigitalLearningSolutions.Data.Extensions;
using DigitalLearningSolutions.Data.Models.DelegateUpload;
using DigitalLearningSolutions.Data.Models.User;
using DigitalLearningSolutions.Data.Utilities;
using DigitalLearningSolutions.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using DigitalLearningSolutions.Data.Models.Centres;

[assembly: InternalsVisibleTo("DigitalLearningSolutions.Web.Tests")]

namespace DigitalLearningSolutions.Web.Services
{
    public interface IDelegateUploadFileService
    {
        public IXLTable OpenDelegatesTable(XLWorkbook workbook);
        public BulkUploadResult ProcessDelegatesFile(IXLTable table, int centreId, DateTime welcomeEmailDate, int lastRowProcessed, int maxRowsToProcess, bool includeUpdatedDelegatesInGroup, bool includeSkippedDelegatesInGroup, int adminId, int? delegateGroupId);
        public BulkUploadResult PreProcessDelegatesFile(IXLTable table);
    }

    public class DelegateUploadFileService : IDelegateUploadFileService
    {
        private readonly IClockUtility clockUtility;
        private readonly IConfiguration configuration;
        private readonly IGroupsService groupsService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IRegistrationService registrationService;
        private readonly ISupervisorDelegateService supervisorDelegateService;
        private readonly IUserDataService userDataService;
        private readonly IUserService userService;

        public DelegateUploadFileService(
            IJobGroupsDataService jobGroupsDataService,
            IUserDataService userDataService,
            IUserService userService,
            IRegistrationService registrationService,
            ISupervisorDelegateService supervisorDelegateService,
            IPasswordResetService passwordResetService,
            IGroupsService groupsService,
            IClockUtility clockUtility,
            IConfiguration configuration
        )
        {
            this.jobGroupsDataService = jobGroupsDataService;
            this.userDataService = userDataService;
            this.userService = userService;
            this.registrationService = registrationService;
            this.supervisorDelegateService = supervisorDelegateService;
            this.passwordResetService = passwordResetService;
            this.groupsService = groupsService;
            this.clockUtility = clockUtility;
            this.configuration = configuration;
        }

        public BulkUploadResult PreProcessDelegatesFile(IXLTable table)
        {
            return PreProcessDelegatesTable(table);
        }

        public BulkUploadResult ProcessDelegatesFile(IXLTable table, int centreId, DateTime welcomeEmailDate, int lastRowProcessed, int maxRowsToProcess, bool includeUpdatedDelegatesInGroup, bool includeSkippedDelegatesInGroup, int adminId, int? delegateGroupId)
        {
            return ProcessDelegatesTable(table, centreId, welcomeEmailDate, lastRowProcessed, maxRowsToProcess, includeUpdatedDelegatesInGroup, includeSkippedDelegatesInGroup, adminId, delegateGroupId);
        }

        public IXLTable OpenDelegatesTable(XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheet(DelegateDownloadFileService.DelegatesSheetName);
            worksheet.Columns(1, 15).Unhide();
            var table = worksheet.Tables.Table(0);
            FixSheetCustomPromptColumnHeaders(table);
            if (!ValidateHeaders(table))
            {
                throw new InvalidHeadersException();
            }
            PopulateJobGroupIdColumn(table);
            return table;
        }

        private void FixSheetCustomPromptColumnHeaders(IXLTable table)
        {
            if (table.ColumnCount() == 15)
            {
                table.Field(5).Name = "Answer1";
                table.Field(6).Name = "Answer2";
                table.Field(7).Name = "Answer3";
                table.Field(8).Name = "Answer4";
                table.Field(9).Name = "Answer5";
                table.Field(10).Name = "Answer6";
            }

        }

        private void PopulateJobGroupIdColumn(IXLTable table)
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var rowCount = table.RowCount();
            for (var i = 2; i <= rowCount; i++)
            {
                var jobGroup = table.Column(5).Cell(i).Value.ToString();
                var JobGroupId = jobGroups.FirstOrDefault(item => item.name == jobGroup).id;
                table.Column(4).Cell(i).Value = JobGroupId;
            }
        }
        internal BulkUploadResult PreProcessDelegatesTable(IXLTable table)
        {
            var jobGroupIds = jobGroupsDataService.GetJobGroupsAlphabetical().Select(item => item.id).ToList();
            var delegateRows = table.Rows().Skip(1).Select(row => new DelegateTableRow(table, row)).ToList();

            foreach (var delegateRow in delegateRows)
            {
                PreProcessDelegateRow(delegateRow, jobGroupIds);
            }

            return new BulkUploadResult(delegateRows);
        }
        internal BulkUploadResult ProcessDelegatesTable(IXLTable table, int centreId, DateTime welcomeEmailDate, int lastRowProcessed, int maxRowsToProcess, bool includeUpdatedDelegatesInGroup, bool includeSkippedDelegatesInGroup, int adminId, int? delegateGroupId)
        {
            var jobGroupIds = jobGroupsDataService.GetJobGroupsAlphabetical().Select(item => item.id).ToList();
            var rowCount = table.Rows().Count();
            int lastRowToProcess;
            if (maxRowsToProcess < rowCount - lastRowProcessed)
            {
                lastRowToProcess = lastRowProcessed + maxRowsToProcess;
            }
            else
            {
                lastRowToProcess = rowCount;
            }
            var delegateRows = table.Rows().Skip(lastRowProcessed).Take(lastRowToProcess - lastRowProcessed).Select(row => new DelegateTableRow(table, row)).ToList();

            foreach (var delegateRow in delegateRows)
            {
                ProcessDelegateRow(centreId, welcomeEmailDate, delegateRow, includeUpdatedDelegatesInGroup, includeSkippedDelegatesInGroup, adminId, delegateGroupId);
            }

            return new BulkUploadResult(delegateRows);
        }

        private void PreProcessDelegateRow(
            DelegateTableRow delegateRow,
            IEnumerable<int> jobGroupIds
            )
        {
            if (!delegateRow.Validate(jobGroupIds))
            {
                return;
            }
            if (string.IsNullOrEmpty(delegateRow.CandidateNumber))
            {
                delegateRow.RowStatus = (bool)delegateRow.Active ? RowStatus.RegisteredActive : RowStatus.RegsiteredInactive;
            }
            else
            {
                delegateRow.RowStatus = (bool)delegateRow.Active ? RowStatus.UpdatedActive : RowStatus.UpdatedInactive;
            }
        }

        private void ProcessDelegateRow(
            int centreId,
            DateTime welcomeEmailDate,
            DelegateTableRow delegateRow,
            bool includeUpdatedDelegatesInGroup,
            bool includeSkippedDelegatesInGroup,
            int adminId,
            int? delegateGroupId
        )
        {
            if (string.IsNullOrEmpty(delegateRow.CandidateNumber))
            {
                if (userService.EmailIsHeldAtCentre(delegateRow.Email, centreId))
                {
                    delegateRow.Error = BulkUploadResult.ErrorReason.EmailAddressInUse;
                    return;
                }
                RegisterDelegate(delegateRow, welcomeEmailDate, centreId, adminId, delegateGroupId);
            }
            else
            {
                var delegateEntity = userDataService.GetDelegateByCandidateNumber(delegateRow.CandidateNumber);

                if (delegateEntity == null)
                {
                    delegateRow.Error = BulkUploadResult.ErrorReason.NoRecordForDelegateId;
                    return;
                }

                if (delegateRow.MatchesDelegateEntity(delegateEntity))
                {
                    delegateRow.RowStatus = RowStatus.Skipped;
                    if (delegateRow.Error == null && (bool)delegateRow.Active && includeSkippedDelegatesInGroup && delegateGroupId != null)
                    {
                        //Add delegate to group
                        groupsService.AddDelegateToGroup((int)delegateGroupId, delegateEntity.DelegateAccount.Id, adminId);
                    }
                    return;
                }

                if (
                    delegateRow.Email != delegateEntity.EmailForCentreNotifications &&
                    userDataService.CentreSpecificEmailIsInUseAtCentre(delegateRow.Email!, centreId)
                )
                {
                    delegateRow.Error = BulkUploadResult.ErrorReason.EmailAddressInUse;
                    return;
                }

                UpdateDelegate(delegateRow, delegateEntity);
                if (delegateRow.Error == null && (bool)delegateRow.Active && includeUpdatedDelegatesInGroup && delegateGroupId != null)
                {
                    //Add delegate to group
                    groupsService.AddDelegateToGroup((int)delegateGroupId, delegateEntity.DelegateAccount.Id, adminId);
                }
            }
        }

        private void UpdateDelegate(DelegateTableRow delegateRow, DelegateEntity delegateEntity)
        {
            try
            {
                userDataService.UpdateUserDetails(
                    delegateRow.FirstName!,
                    delegateRow.LastName!,
                    delegateEntity.UserAccount.PrimaryEmail,
                    delegateRow.JobGroupId!.Value,
                    delegateEntity.UserAccount.Id
                );

                userDataService.UpdateDelegateAccount(
                    delegateEntity.DelegateAccount.Id,
                    delegateRow.Active!.Value,
                    delegateRow.Answer1,
                    delegateRow.Answer2,
                    delegateRow.Answer3,
                    delegateRow.Answer4,
                    delegateRow.Answer5,
                    delegateRow.Answer6
                );

                UpdateUserProfessionalRegistrationNumberIfNecessary(
                    delegateRow.HasPrn,
                    delegateRow.Prn,
                    delegateEntity.DelegateAccount.Id
                );

                if (!string.Equals(delegateEntity.EmailForCentreNotifications, delegateRow.Email))
                {
                    userDataService.SetCentreEmail(
                        delegateEntity.UserAccount.Id,
                        delegateEntity.DelegateAccount.CentreId,
                        delegateRow.Email,
                        clockUtility.UtcNow
                    );
                }

                groupsService.UpdateSynchronisedDelegateGroupsBasedOnUserChanges(
                    delegateEntity.DelegateAccount.Id,
                    new AccountDetailsData(
                        delegateRow.FirstName!,
                        delegateRow.LastName!,
                        delegateEntity.UserAccount.PrimaryEmail
                    ),
                    new RegistrationFieldAnswers(
                        delegateEntity.DelegateAccount.CentreId,
                        delegateRow.JobGroupId.Value,
                        delegateRow.Answer1,
                        delegateRow.Answer2,
                        delegateRow.Answer3,
                        delegateRow.Answer4,
                        delegateRow.Answer5,
                        delegateRow.Answer6
                    ),
                    new RegistrationFieldAnswers(
                        delegateEntity.DelegateAccount.CentreId,
                        delegateEntity.UserAccount.JobGroupId,
                        delegateEntity.DelegateAccount.Answer1,
                        delegateEntity.DelegateAccount.Answer2,
                        delegateEntity.DelegateAccount.Answer3,
                        delegateEntity.DelegateAccount.Answer4,
                        delegateEntity.DelegateAccount.Answer5,
                        delegateEntity.DelegateAccount.Answer6
                    ),
                    delegateRow.Email
                );

                delegateRow.RowStatus = (bool)delegateRow.Active ? RowStatus.UpdatedActive : RowStatus.UpdatedInactive;
            }
            catch
            {
                delegateRow.Error = BulkUploadResult.ErrorReason.UnexpectedErrorForUpdate;
            }
        }

        private void RegisterDelegate(DelegateTableRow delegateTableRow, DateTime welcomeEmailDate, int centreId, int adminId, int? delegateGroupId)
        {
            var model = RegistrationMappingHelper.MapDelegateUploadTableRowToDelegateRegistrationModel(delegateTableRow, welcomeEmailDate, centreId);

            var (delegateId, _, delegateUserId) =
                registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(model, false, true);

            UpdateUserProfessionalRegistrationNumberIfNecessary(
                delegateTableRow.HasPrn,
                delegateTableRow.Prn,
                delegateId
            );

            SetUpSupervisorDelegateRelations(delegateTableRow.Email!, centreId, delegateUserId);

            passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                delegateId,
                configuration.GetAppRootPath(),
                welcomeEmailDate,
                "DelegateBulkUpload_Refactor"
            );
            if (delegateGroupId != null && (bool)delegateTableRow.Active)
            {
                //Add delegate to group
                groupsService.AddDelegateToGroup((int)delegateGroupId, delegateId, adminId);
            }
            delegateTableRow.RowStatus = (bool)delegateTableRow.Active ? RowStatus.RegisteredActive : RowStatus.RegsiteredInactive;
        }

        private void UpdateUserProfessionalRegistrationNumberIfNecessary(
            bool? delegateRowHasPrn,
            string? delegateRowPrn,
            int delegateId
        )
        {
            if (delegateRowPrn != null)
            {
                userDataService.UpdateDelegateProfessionalRegistrationNumber(
                    delegateId,
                    delegateRowPrn,
                    true
                );
            }
            else
            {
                userDataService.UpdateDelegateProfessionalRegistrationNumber(
                    delegateId,
                    null,
                    delegateRowHasPrn.HasValue
                );
            }
        }

        private void SetUpSupervisorDelegateRelations(string emailAddress, int centreId, int delegateUserId)
        {
            var pendingSupervisorDelegateIds =
                supervisorDelegateService.GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
                    centreId,
                    new List<string?> { emailAddress }
                ).Select(supervisor => supervisor.ID).ToList();

            if (!pendingSupervisorDelegateIds.Any())
            {
                return;
            }

            // TODO: HEEDLS-1014 - Change Delegate ID to User ID
            supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                pendingSupervisorDelegateIds,
                delegateUserId
            );
        }

        private static bool ValidateHeaders(IXLTable table)
        {
            var expectedHeaders = new List<string>
            {
                "DelegateID",
                "LastName",
                "FirstName",
                "JobGroupID",
                "JobGroup",
                "Answer1",
                "Answer2",
                "Answer3",
                "Answer4",
                "Answer5",
                "Answer6",
                "Active",
                "EmailAddress",
                "HasPRN",
                "PRN",
            }.OrderBy(x => x);
            var actualHeaders = table.Fields.Select(x => x.Name).OrderBy(x => x);
            return actualHeaders.SequenceEqual(expectedHeaders);
        }
    }
}
