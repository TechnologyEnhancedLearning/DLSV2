namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Register;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    public interface IRegistrationService
    {
        (string candidateNumber, bool approved) RegisterDelegate(
            DelegateRegistrationModel delegateRegistrationModel,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            int? inviteId = null
        );

        string RegisterDelegateByCentre(DelegateRegistrationModel delegateRegistrationModel, string baseUrl);

        void RegisterCentreManager(
            AdminRegistrationModel registrationModel,
            int jobGroupId,
            bool registerJourneyContainsTermsAndConditions
        );

        void PromoteDelegateToAdmin(AdminRoles adminRoles, int categoryId, int delegateId);
    }

    public class RegistrationService : IRegistrationService
    {
        private readonly ICentresDataService centresDataService;
        private readonly IConfiguration config;
        private readonly IEmailService emailService;
        private readonly ILogger<RegistrationService> logger;
        private readonly IPasswordDataService passwordDataService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IRegistrationDataService registrationDataService;
        private readonly ISupervisorDelegateService supervisorDelegateService;
        private readonly IUserDataService userDataService;
        private readonly INotificationDataService notificationDataService;

        public RegistrationService(
            IRegistrationDataService registrationDataService,
            IPasswordDataService passwordDataService,
            IPasswordResetService passwordResetService,
            IEmailService emailService,
            ICentresDataService centresDataService,
            IConfiguration config,
            ISupervisorDelegateService supervisorDelegateService,
            IUserDataService userDataService,
            INotificationDataService notificationDataService,
            ILogger<RegistrationService> logger
        )
        {
            this.registrationDataService = registrationDataService;
            this.passwordDataService = passwordDataService;
            this.passwordResetService = passwordResetService;
            this.emailService = emailService;
            this.centresDataService = centresDataService;
            this.userDataService = userDataService;
            this.config = config;
            this.supervisorDelegateService = supervisorDelegateService;
            this.userDataService = userDataService;
            this.notificationDataService = notificationDataService;
            this.logger = logger;
        }

        public (string candidateNumber, bool approved) RegisterDelegate(
            DelegateRegistrationModel delegateRegistrationModel,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            int? supervisorDelegateId = null
        )
        {
            var supervisorDelegateRecordIdsMatchingDelegate =
                GetPendingSupervisorDelegateIdsMatchingDelegate(delegateRegistrationModel).ToList();

            var foundRecordForSupervisorDelegateId = supervisorDelegateId.HasValue &&
                                                     supervisorDelegateRecordIdsMatchingDelegate.Contains(
                                                         supervisorDelegateId.Value
                                                     );

            var centreIpPrefixes = centresDataService.GetCentreIpPrefixes(delegateRegistrationModel.Centre);
            delegateRegistrationModel.Approved = foundRecordForSupervisorDelegateId ||
                                                 centreIpPrefixes.Any(ip => userIp.StartsWith(ip.Trim())) ||
                                                 userIp == "::1";

            var candidateNumber = CreateAccountAndReturnCandidateNumber(delegateRegistrationModel);

            passwordDataService.SetPasswordByCandidateNumber(
                candidateNumber,
                delegateRegistrationModel.PasswordHash!
            );

            // We know this will give us a non-null user.
            // If the delegate hadn't successfully been added we would have errored out of this method earlier.
            var delegateUser = userDataService.GetDelegateUserByCandidateNumber(
                candidateNumber,
                delegateRegistrationModel.Centre
            )!;

            userDataService.UpdateDelegateProfessionalRegistrationNumber(
                delegateUser.Id,
                delegateRegistrationModel.ProfessionalRegistrationNumber,
                true
            );

            if (supervisorDelegateRecordIdsMatchingDelegate.Any())
            {
                supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    supervisorDelegateRecordIdsMatchingDelegate,
                    delegateUser.Id
                );
            }

            if (!delegateRegistrationModel.Approved)
            {
                var recipients = notificationDataService.GetAdminRecipientsForCentreNotification(delegateRegistrationModel.Centre, 4);

                foreach (var recipient in recipients)
                {
                    if (recipient.Email != null && recipient.FirstName != null)
                    {
                        var approvalEmail = GenerateApprovalEmail(
                        recipient.Email,
                        recipient.FirstName,
                        delegateRegistrationModel.FirstName,
                        delegateRegistrationModel.LastName,
                        refactoredTrackingSystemEnabled
                    );
                        emailService.SendEmail(approvalEmail);
                    }
                }
            }

            return (candidateNumber, delegateRegistrationModel.Approved);
        }

        public void PromoteDelegateToAdmin(AdminRoles adminRoles, int categoryId, int delegateId)
        {
            var delegateUser = userDataService.GetDelegateUserById(delegateId)!;

            if (string.IsNullOrWhiteSpace(delegateUser.EmailAddress) ||
                string.IsNullOrWhiteSpace(delegateUser.FirstName) ||
                string.IsNullOrWhiteSpace(delegateUser.Password))
            {
                throw new AdminCreationFailedException(
                    "Delegate missing first name, email or password",
                    AdminCreationError.UnexpectedError
                );
            }

            var adminUser = userDataService.GetAdminUserByEmailAddress(delegateUser.EmailAddress);

            if (adminUser?.Active == false && adminUser.CentreId == delegateUser.CentreId)
            {
                userDataService.ReactivateAdmin(adminUser.Id);
                userDataService.UpdateAdminUser(
                    delegateUser.FirstName,
                    delegateUser.LastName,
                    delegateUser.EmailAddress,
                    delegateUser.ProfileImage,
                    adminUser.Id
                );
                passwordDataService.SetPasswordByAdminId(adminUser.Id, delegateUser.Password);
                userDataService.UpdateAdminUserPermissions(
                    adminUser.Id,
                    adminRoles.IsCentreAdmin,
                    adminRoles.IsSupervisor,
                    adminRoles.IsNominatedSupervisor,
                    adminRoles.IsTrainer,
                    adminRoles.IsContentCreator,
                    adminRoles.IsContentManager,
                    adminRoles.ImportOnly,
                    categoryId
                );
            }
            else if (adminUser?.Active == true && adminUser.CentreId == delegateUser.CentreId)
            {
                adminRoles.IsCentreAdmin = adminRoles.IsCentreAdmin || adminUser.IsCentreAdmin;
                adminRoles.IsContentCreator = adminRoles.IsContentCreator || adminUser.IsContentCreator;
                adminRoles.IsContentManager = adminRoles.IsContentManager || adminUser.IsContentManager;
                adminRoles.IsTrainer = adminRoles.IsTrainer || adminUser.IsTrainer;
                adminRoles.IsSupervisor = adminRoles.IsSupervisor || adminUser.IsSupervisor;
                adminRoles.IsNominatedSupervisor = adminRoles.IsNominatedSupervisor || adminUser.IsNominatedSupervisor;
                adminRoles.ImportOnly = adminRoles.ImportOnly || adminUser.ImportOnly;
                userDataService.UpdateAdminUserPermissions(
                    adminUser.Id,
                    adminRoles.IsCentreAdmin,
                    adminRoles.IsSupervisor,
                    adminRoles.IsNominatedSupervisor,
                    adminRoles.IsTrainer,
                    adminRoles.IsContentCreator,
                    adminRoles.IsContentManager,
                    adminRoles.ImportOnly,
                    categoryId
                );
            }
            else if (adminUser == null)
            {
                var adminRegistrationModel = new AdminRegistrationModel(
                    delegateUser.FirstName,
                    delegateUser.LastName,
                    delegateUser.EmailAddress,
                    delegateUser.CentreId,
                    delegateUser.Password,
                    true,
                    true,
                    delegateUser.ProfessionalRegistrationNumber,
                    categoryId,
                    adminRoles.IsCentreAdmin,
                    false,
                    adminRoles.IsSupervisor,
                    adminRoles.IsNominatedSupervisor,
                    adminRoles.IsTrainer,
                    adminRoles.IsContentCreator,
                    adminRoles.IsCmsAdministrator,
                    adminRoles.IsCmsManager,
                    delegateUser.ProfileImage
                );

                registrationDataService.RegisterAdmin(adminRegistrationModel, false);
            }
            else
            {
                throw new AdminCreationFailedException(AdminCreationError.EmailAlreadyInUse);
            }
        }

        public string RegisterDelegateByCentre(DelegateRegistrationModel delegateRegistrationModel, string baseUrl)
        {
            using var transaction = new TransactionScope();

            var candidateNumber = CreateAccountAndReturnCandidateNumber(delegateRegistrationModel);

            if (delegateRegistrationModel.PasswordHash != null)
            {
                passwordDataService.SetPasswordByCandidateNumber(
                    candidateNumber,
                    delegateRegistrationModel.PasswordHash
                );
            }
            else if (delegateRegistrationModel.NotifyDate.HasValue)
            {
                passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    delegateRegistrationModel.Email,
                    candidateNumber,
                    baseUrl,
                    delegateRegistrationModel.NotifyDate.Value,
                    "RegisterDelegateByCentre_Refactor"
                );
            }

            // We know this will give us a non-null user.
            // If the delegate hadn't successfully been added we would have errored out of this method earlier.
            var delegateUser = userDataService.GetDelegateUserByCandidateNumber(
                candidateNumber,
                delegateRegistrationModel.Centre
            )!;

            userDataService.UpdateDelegateProfessionalRegistrationNumber(
                delegateUser.Id,
                delegateRegistrationModel.ProfessionalRegistrationNumber,
                true
            );

            var supervisorDelegateRecordIdsMatchingDelegate =
                GetPendingSupervisorDelegateIdsMatchingDelegate(delegateRegistrationModel).ToList();
            if (supervisorDelegateRecordIdsMatchingDelegate.Any())
            {
                supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    supervisorDelegateRecordIdsMatchingDelegate,
                    delegateUser.Id
                );
            }

            transaction.Complete();

            return candidateNumber;
        }

        public void RegisterCentreManager(
            AdminRegistrationModel registrationModel,
            int jobGroupId,
            bool registerJourneyContainsTermsAndConditions
        )
        {
            using var transaction = new TransactionScope();

            CreateDelegateAccountForAdmin(registrationModel, jobGroupId);

            registrationDataService.RegisterAdmin(registrationModel, registerJourneyContainsTermsAndConditions);

            centresDataService.SetCentreAutoRegistered(registrationModel.Centre);

            transaction.Complete();
        }

        private string CreateAccountAndReturnCandidateNumber(DelegateRegistrationModel delegateRegistrationModel)
        {
            var candidateNumberOrErrorCode = registrationDataService.RegisterDelegate(delegateRegistrationModel);

            var failureIfAny = DelegateCreationError.FromStoredProcedureErrorCode(candidateNumberOrErrorCode);

            if (failureIfAny != null)
            {
                logger.LogError(
                    $"Could not create account for delegate on registration. Failure: {failureIfAny.Name}."
                );

                throw new DelegateCreationFailedException(failureIfAny);
            }

            return candidateNumberOrErrorCode;
        }

        private IEnumerable<int> GetPendingSupervisorDelegateIdsMatchingDelegate(
            DelegateRegistrationModel delegateRegistrationModel
        )
        {
            return supervisorDelegateService
                .GetPendingSupervisorDelegateRecordsByEmailAndCentre(
                    delegateRegistrationModel.Centre,
                    delegateRegistrationModel.Email
                ).Select(record => record.ID);
        }

        private void CreateDelegateAccountForAdmin(AdminRegistrationModel registrationModel, int jobGroupId)
        {
            var delegateRegistrationModel = new DelegateRegistrationModel(
                registrationModel.FirstName,
                registrationModel.LastName,
                registrationModel.Email,
                registrationModel.Centre,
                jobGroupId,
                registrationModel.PasswordHash!,
                true,
                true,
                registrationModel.ProfessionalRegistrationNumber
            );

            var candidateNumberOrErrorCode = registrationDataService.RegisterDelegate(delegateRegistrationModel);
            var failureIfAny = DelegateCreationError.FromStoredProcedureErrorCode(candidateNumberOrErrorCode);
            if (failureIfAny != null)
            {
                logger.LogError(
                    $"Delegate account could not be created (error code: {candidateNumberOrErrorCode}) with email address: {registrationModel.Email}"
                );

                throw new DelegateCreationFailedException(failureIfAny);
            }

            passwordDataService.SetPasswordByCandidateNumber(
                candidateNumberOrErrorCode,
                delegateRegistrationModel.PasswordHash!
            );

            // We know this will give us a non-null user.
            // If the delegate hadn't successfully been added we would have errored out of this method earlier.
            var delegateUser = userDataService.GetDelegateUserByCandidateNumber(
                candidateNumberOrErrorCode,
                delegateRegistrationModel.Centre
            )!;

            userDataService.UpdateDelegateProfessionalRegistrationNumber(
                delegateUser.Id,
                registrationModel.ProfessionalRegistrationNumber,
                true
            );
        }

        private Email GenerateApprovalEmail(
            string emailAddress,
            string firstName,
            string learnerFirstName,
            string learnerLastName,
            bool refactoredTrackingSystemEnabled
        )
        {
            const string emailSubject = "Digital Learning Solutions Registration Requires Approval";
            var approvalUrl = refactoredTrackingSystemEnabled
                ? $"{config["AppRootPath"]}/TrackingSystem/Delegates/Approve"
                : $"{config["CurrentSystemBaseUrl"]}/tracking/approvedelegates";

            var body = new BodyBuilder
            {
                TextBody = $@"Dear {firstName},
                            A learner, {learnerFirstName} {learnerLastName}, has registered against your Digital Learning Solutions centre and requires approval before they can access courses.
                            To approve or reject their registration please follow this link: {approvalUrl}
                            Please don't reply to this email as it has been automatically generated.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear {firstName},</p>
                                <p>A learner, {learnerFirstName} {learnerLastName}, has registered against your Digital Learning Solutions centre and requires approval before they can access courses.</p>
                                <p>To approve or reject their registration please follow this link: <a href=""{approvalUrl}"">{approvalUrl}</a></p>
                                <p>Please don't reply to this email as it has been automatically generated.</p>
                            </body>",
            };

            return new Email(emailSubject, body, emailAddress);
        }
    }
}
