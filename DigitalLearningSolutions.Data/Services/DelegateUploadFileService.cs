using System.Runtime.CompilerServices;

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
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    public interface IDelegateUploadFileService
    {
        public BulkUploadResult ProcessDelegatesFile(IFormFile file, int centreId, DateTime welcomeEmailDate);
    }

    public class DelegateUploadFileService : IDelegateUploadFileService
    {
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserDataService userDataService;
        private readonly IRegistrationService registrationService;
        private readonly ISupervisorDelegateService supervisorDelegateService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IGroupsService groupsService;
        private readonly IConfiguration configuration;

        public DelegateUploadFileService(
            IJobGroupsDataService jobGroupsDataService,
            IUserDataService userDataService,
            IRegistrationService registrationService,
            ISupervisorDelegateService supervisorDelegateService,
            IPasswordResetService passwordResetService,
            IConfiguration configuration,
            IGroupsService groupsService
        )
        {
            this.jobGroupsDataService = jobGroupsDataService;
            this.userDataService = userDataService;
            this.registrationService = registrationService;
            this.supervisorDelegateService = supervisorDelegateService;
            this.passwordResetService = passwordResetService;
            this.configuration = configuration;
            this.groupsService = groupsService;
        }

        public BulkUploadResult ProcessDelegatesFile(IFormFile file, int centreId, DateTime welcomeEmailDate)
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

        internal BulkUploadResult ProcessDelegatesTable(IXLTable table, int centreId, DateTime welcomeEmailDate)
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
            DateTime welcomeEmailDate,
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
                if (userDataService.CentreSpecificEmailIsInUseAtCentre(delegateRow.Email!, centreId))
                {
                    delegateRow.Error = BulkUploadResult.ErrorReason.EmailAddressInUse;
                    return;
                }

                RegisterDelegate(delegateRow, welcomeEmailDate, centreId);
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

                if (delegateRow.Email != delegateEntity.EmailForCentreNotifications)
                {
                    userDataService.SetCentreEmail(
                        delegateEntity.UserAccount.Id,
                        delegateEntity.DelegateAccount.CentreId,
                        delegateRow.Email
                    );
                }

                delegateRow.RowStatus = RowStatus.Updated;
            }
            catch
            {
                delegateRow.Error = BulkUploadResult.ErrorReason.UnexpectedErrorForUpdate;
            }
        }

        private void RegisterDelegate(DelegateTableRow delegateRow, DateTime welcomeEmailDate, int centreId)
        {
            var model = new DelegateRegistrationModel(delegateRow, centreId, welcomeEmailDate);

            var (delegateId, _) = registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(model, false);

            UpdateUserProfessionalRegistrationNumberIfNecessary(
                delegateRow.HasPrn,
                delegateRow.Prn,
                delegateId
            );

            groupsService.AddNewDelegateToRegistrationFieldGroupsAndEnrolOnCourses(
                delegateId,
                model.GetRegistrationFieldAnswers()
            );

            SetUpSupervisorDelegateRelations(delegateRow.Email!, centreId, delegateId);

            passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                delegateId,
                configuration.GetAppRootPath(),
                welcomeEmailDate,
                "DelegateBulkUpload_Refactor"
            );

            delegateRow.RowStatus = RowStatus.Registered;
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

        private void SetUpSupervisorDelegateRelations(string emailAddress, int centreId, int delegateId)
        {
            var pendingSupervisorDelegateIds =
                supervisorDelegateService.GetPendingSupervisorDelegateRecordsByEmailAndCentre(
                    centreId,
                    emailAddress
                ).Select(supervisor => supervisor.ID).ToList();

            if (!pendingSupervisorDelegateIds.Any())
            {
                return;
            }

            supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                pendingSupervisorDelegateIds,
                delegateId
            );
        }

        private static bool ValidateHeaders(IXLTable table)
        {
            var expectedHeaders = new List<string>
            {
                "LastName",
                "FirstName",
                "DelegateID",
                "JobGroupID",
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
