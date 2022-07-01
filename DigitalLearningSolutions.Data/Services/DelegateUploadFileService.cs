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
        public BulkUploadResult ProcessDelegatesFile(IFormFile file, int centreId, DateTime? welcomeEmailDate = null);
    }

    public class DelegateUploadFileService : IDelegateUploadFileService
    {
        private readonly IConfiguration configuration;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IRegistrationService registrationService;
        private readonly ISupervisorDelegateService supervisorDelegateService;
        private readonly IUserDataService userDataService;
        private readonly IUserService userService;

        public DelegateUploadFileService(
            IJobGroupsDataService jobGroupsDataService,
            IUserDataService userDataService,
            IRegistrationService registrationService,
            ISupervisorDelegateService supervisorDelegateService,
            IUserService userService,
            IPasswordResetService passwordResetService,
            IConfiguration configuration
        )
        {
            this.userDataService = userDataService;
            this.registrationService = registrationService;
            this.supervisorDelegateService = supervisorDelegateService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.userService = userService;
            this.passwordResetService = passwordResetService;
            this.configuration = configuration;
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

            if (string.IsNullOrEmpty(delegateRow.CandidateNumber))
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
                var delegateEntity = userDataService.GetDelegateByCandidateNumber(delegateRow.CandidateNumber);

                if (delegateEntity == null)
                {
                    delegateRow.Error = BulkUploadResult.ErrorReason.NoRecordForDelegateId;
                    return;
                }

                if (delegateRow.Email != delegateEntity.UserAccount.PrimaryEmail &&
                    !userService.IsDelegateEmailValidForCentre(delegateRow.Email!, centreId)
                )
                {
                    delegateRow.Error = BulkUploadResult.ErrorReason.EmailAddressInUse;
                    return;
                }

                if (delegateRow.MatchesDelegateEntity(delegateEntity))
                {
                    delegateRow.RowStatus = RowStatus.Skipped;
                    return;
                }

                UpdateDelegate(delegateRow, delegateEntity);
            }
        }

        // TODO HEEDLS-887 Make sure this logic is correct with the new account structure
        private void UpdateDelegate(DelegateTableRow delegateRow, DelegateEntity delegateEntity)
        {
            try
            {
                userDataService.UpdateUserDetails(
                    delegateRow.FirstName!,
                    delegateRow.LastName!,
                    delegateRow.Email!,
                    delegateRow.JobGroupId!.Value,
                    1 // TODO HEEDLS-887 This needs correcting to the correct UserId for the delegate record.
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

            var (delegateId, _) = registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(model, false);

            UpdateUserProfessionalRegistrationNumberIfNecessary(
                delegateRow.HasPrn,
                delegateRow.Prn,
                delegateId
            );

            SetUpSupervisorDelegateRelations(delegateRow.Email!, centreId, delegateId);

            if (welcomeEmailDate.HasValue)
            {
                passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    delegateId,
                    configuration.GetAppRootPath(),
                    welcomeEmailDate.Value,
                    "DelegateBulkUpload_Refactor"
                );
            }

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
