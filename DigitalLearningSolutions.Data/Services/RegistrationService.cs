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
        (string candidateNumber, bool approved) CreateDelegateAccountForNewUser(
            DelegateRegistrationModel delegateRegistrationModel,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            int? inviteId = null
        );

        (string candidateNumber, bool approved) CreateDelegateAccountForExistingUser(
            InternalDelegateRegistrationModel delegateRegistrationModel,
            int userId,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            int? supervisorDelegateId = null
        );

        string RegisterDelegateByCentre(DelegateRegistrationModel delegateRegistrationModel, string baseUrl);

        void RegisterCentreManager(AdminRegistrationModel registrationModel, int jobGroupId);

        void PromoteDelegateToAdmin(AdminRoles adminRoles, int? categoryId, int delegateId);

        string CreateAccountAndReturnCandidateNumber(
            DelegateRegistrationModel delegateRegistrationModel,
            int? userId = null
        );
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
        private readonly IUserService userService;

        public RegistrationService(
            IRegistrationDataService registrationDataService,
            IPasswordDataService passwordDataService,
            IPasswordResetService passwordResetService,
            IEmailService emailService,
            ICentresDataService centresDataService,
            IConfiguration config,
            ISupervisorDelegateService supervisorDelegateService,
            IUserDataService userDataService,
            ILogger<RegistrationService> logger,
            IUserService userService
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
            this.logger = logger;
            this.userService = userService;
        }

        public (string candidateNumber, bool approved) CreateDelegateAccountForNewUser(
            DelegateRegistrationModel delegateRegistrationModel,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            int? supervisorDelegateId = null
        )
        {
            // TODO HEEDLS-899 it's undecided at time of comment whether this should be matched on centre email or primary email
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
                var contactInfo = centresDataService.GetCentreManagerDetails(delegateRegistrationModel.Centre);
                var approvalEmail = GenerateApprovalEmail(
                    contactInfo.email,
                    contactInfo.firstName,
                    delegateRegistrationModel.FirstName,
                    delegateRegistrationModel.LastName,
                    refactoredTrackingSystemEnabled
                );
                emailService.SendEmail(approvalEmail);
            }

            return (candidateNumber, delegateRegistrationModel.Approved);
        }

        public (string candidateNumber, bool approved) CreateDelegateAccountForExistingUser(
            InternalDelegateRegistrationModel internalDelegateRegistrationModel,
            int userId,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            int? supervisorDelegateId = null
        )
        {
            var userAccount = userService.GetUserById(userId)!;

            var delegateRegistrationModel =
                new DelegateRegistrationModel(userAccount.UserAccount, internalDelegateRegistrationModel);

            // TODO HEEDLS-899 sort out supervisor delegate stuff
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

            var candidateNumber = CreateAccountAndReturnCandidateNumber(delegateRegistrationModel, userId);
            
            // We know this will give us a non-null user.
            // If the delegate hadn't successfully been added we would have errored out of this method earlier.
            var delegateUser = userDataService.GetDelegateUserByCandidateNumber(
                candidateNumber,
                delegateRegistrationModel.Centre
            )!;

            if (supervisorDelegateRecordIdsMatchingDelegate.Any())
            {
                supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    supervisorDelegateRecordIdsMatchingDelegate,
                    delegateUser.Id
                );
            }

            if (!delegateRegistrationModel.Approved)
            {
                var contactInfo = centresDataService.GetCentreManagerDetails(delegateRegistrationModel.Centre);
                var approvalEmail = GenerateApprovalEmail(
                    contactInfo.email,
                    contactInfo.firstName,
                    userAccount.UserAccount.FirstName,
                    userAccount.UserAccount.LastName,
                    refactoredTrackingSystemEnabled
                );
                emailService.SendEmail(approvalEmail);
            }

            return (candidateNumber, delegateRegistrationModel.Approved);
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
                    delegateRegistrationModel.PrimaryEmail,
                    candidateNumber,
                    baseUrl,
                    delegateRegistrationModel.NotifyDate.Value,
                    "RegisterDelegateByCentre_Refactor"
                );
            }

            // TODO HEEDLS-899 it's undecided at time of comment whether this should be matched on centre email or primary email
            var supervisorDelegateRecordIdsMatchingDelegate =
                GetPendingSupervisorDelegateIdsMatchingDelegate(delegateRegistrationModel).ToList();

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

            transaction.Complete();

            return candidateNumber;
        }

        public void RegisterCentreManager(AdminRegistrationModel registrationModel, int jobGroupId)
        {
            using var transaction = new TransactionScope();

            CreateDelegateAccountForAdmin(registrationModel, jobGroupId);

            // TODO HEEDLS-900 these user IDs are placeholders and should be updated
            registrationDataService.RegisterAdmin(registrationModel, 0);

            centresDataService.SetCentreAutoRegistered(registrationModel.Centre);

            transaction.Complete();
        }

        public void PromoteDelegateToAdmin(AdminRoles adminRoles, int? categoryId, int delegateId)
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
            
            if (adminUser != null)
            {
                throw new AdminCreationFailedException(AdminCreationError.EmailAlreadyInUse);
            }

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

            // TODO HEEDLS-900 these user IDs are placeholders and should be updated
            registrationDataService.RegisterAdmin(adminRegistrationModel, 0);
        }

        public string CreateAccountAndReturnCandidateNumber(
            DelegateRegistrationModel delegateRegistrationModel,
            int? userId = null
        )
        {
            try
            {
                ValidateRegistrationEmail(delegateRegistrationModel);
                return registrationDataService.RegisterNewUserAndDelegateAccount(delegateRegistrationModel, userId);
            }
            catch (DelegateCreationFailedException e)
            {
                var error = e.Error;
                logger.LogError(
                    $"Could not create account for delegate on registration. Failure: {error.Name}"
                );
                throw new DelegateCreationFailedException(error);
            }
            catch
            {
                var error = DelegateCreationError.UnexpectedError;
                logger.LogError(
                    $"Could not create account for delegate on registration. Failure: {error.Name}"
                );
                throw new DelegateCreationFailedException(error);
            }
        }

        private void ValidateRegistrationEmail(DelegateRegistrationModel model)
        {
            var emails = (IEnumerable<string>)new[] { model.PrimaryEmail, model.SecondaryEmail }.Where(e => e != null);
            if (userDataService.AnyEmailsInSetAreAlreadyInUse(emails))
            {
                var error = DelegateCreationError.EmailAlreadyInUse;
                logger.LogError(
                    $"Could not create account for delegate on registration. Failure: {error.Name}."
                );
                throw new DelegateCreationFailedException(error);
            }
        }

        private IEnumerable<int> GetPendingSupervisorDelegateIdsMatchingDelegate(
            DelegateRegistrationModel delegateRegistrationModel
        )
        {
            return supervisorDelegateService
                .GetPendingSupervisorDelegateRecordsByEmailAndCentre(
                    delegateRegistrationModel.Centre,
                    // TODO HEEDLS-899 it's undecided at time of comment whether this should be matched on centre email or primary email
                    delegateRegistrationModel.PrimaryEmail
                ).Select(record => record.ID);
        }

        private void CreateDelegateAccountForAdmin(AdminRegistrationModel registrationModel, int jobGroupId)
        {
            var delegateRegistrationModel = new DelegateRegistrationModel(
                registrationModel.FirstName,
                registrationModel.LastName,
                registrationModel.PrimaryEmail,
                registrationModel.Centre,
                jobGroupId,
                registrationModel.PasswordHash!,
                true,
                true,
                registrationModel.ProfessionalRegistrationNumber
            );

            var candidateNumberOrErrorCode =
                registrationDataService.RegisterNewUserAndDelegateAccount(delegateRegistrationModel);
            var failureIfAny = DelegateCreationError.FromStoredProcedureErrorCode(candidateNumberOrErrorCode);
            if (failureIfAny != null)
            {
                logger.LogError(
                    $"Delegate account could not be created (error code: {candidateNumberOrErrorCode}) with email address: {registrationModel.PrimaryEmail}"
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
