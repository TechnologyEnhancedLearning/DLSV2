namespace DigitalLearningSolutions.Data.Services
{
    using System;
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

        (string candidateNumber, bool approved, bool userHasAdminAccountAtCentre) CreateDelegateAccountForExistingUser(
            InternalDelegateRegistrationModel internalDelegateRegistrationModel,
            int userId,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            int? supervisorDelegateId = null
        );

        string RegisterDelegateByCentre(DelegateRegistrationModel delegateRegistrationModel, string baseUrl);

        void RegisterCentreManager(AdminRegistrationModel registrationModel);

        void PromoteDelegateToAdmin(AdminRoles adminRoles, int? categoryId, int delegateId);

        string CreateAccountAndReturnCandidateNumber(DelegateRegistrationModel delegateRegistrationModel);
    }

    public class RegistrationService : IRegistrationService
    {
        private readonly ICentresDataService centresDataService;
        private readonly IClockService clockService;
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
            IUserService userService,
            IClockService clockService
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
            this.clockService = clockService;
        }

        public (string candidateNumber, bool approved) CreateDelegateAccountForNewUser(
            DelegateRegistrationModel delegateRegistrationModel,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            int? supervisorDelegateId = null
        )
        {
            // TODO HEEDLS-899 sort out supervisor delegate stuff
            var supervisorDelegateRecordIdsMatchingDelegate =
                GetPendingSupervisorDelegateIdsMatchingDelegate(delegateRegistrationModel).ToList();

            delegateRegistrationModel.Approved = NewDelegateAccountShouldBeApproved(
                userIp,
                supervisorDelegateId,
                supervisorDelegateRecordIdsMatchingDelegate,
                delegateRegistrationModel.Centre
            );

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

            SendDelegateNeedsApprovalEmailIfNecessary(delegateRegistrationModel, refactoredTrackingSystemEnabled);

            return (candidateNumber, delegateRegistrationModel.Approved);
        }

        public (string candidateNumber, bool approved, bool userHasAdminAccountAtCentre)
            CreateDelegateAccountForExistingUser(
                InternalDelegateRegistrationModel internalDelegateRegistrationModel,
                int userId,
                string userIp,
                bool refactoredTrackingSystemEnabled,
                int? supervisorDelegateId = null
            )
        {
            var userEntity = userService.GetUserById(userId)!;

            var delegateRegistrationModel =
                new DelegateRegistrationModel(userEntity.UserAccount, internalDelegateRegistrationModel);

            var userHasAdminAccountAtCentre = userEntity.GetCentreAccountSet(internalDelegateRegistrationModel.Centre)
                ?.CanLogIntoAdminAccount == true;

            // TODO HEEDLS-899 sort out supervisor delegate stuff, this is just copied from the external registration
            var supervisorDelegateRecordIdsMatchingDelegate =
                GetPendingSupervisorDelegateIdsMatchingDelegate(delegateRegistrationModel).ToList();

            delegateRegistrationModel.Approved = NewDelegateAccountShouldBeApproved(
                userIp,
                supervisorDelegateId,
                supervisorDelegateRecordIdsMatchingDelegate,
                delegateRegistrationModel.Centre,
                userHasAdminAccountAtCentre
            );

            var (delegateId, candidateNumber) =
                RegisterDelegateAccountAndCentreDetailsForExistingUser(userId, delegateRegistrationModel);

            if (supervisorDelegateRecordIdsMatchingDelegate.Any())
            {
                supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    supervisorDelegateRecordIdsMatchingDelegate,
                    delegateId
                );
            }

            SendDelegateNeedsApprovalEmailIfNecessary(delegateRegistrationModel, refactoredTrackingSystemEnabled);

            return (candidateNumber, delegateRegistrationModel.Approved, userHasAdminAccountAtCentre);
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

            // TODO HEEDLS-899 sort out supervisor delegate stuff
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

        public void RegisterCentreManager(AdminRegistrationModel registrationModel)
        {
            using var transaction = new TransactionScope();

            var userId = CreateDelegateAccountForAdmin(registrationModel);

            registrationDataService.RegisterAdmin(registrationModel, userId);

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
                null,
                delegateUser.CentreId,
                delegateUser.Password,
                true,
                true,
                delegateUser.ProfessionalRegistrationNumber,
                delegateUser.JobGroupId,
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

            var userId = userDataService.GetUserIdFromDelegateId(delegateId);
            registrationDataService.RegisterAdmin(adminRegistrationModel, userId);
        }

        public string CreateAccountAndReturnCandidateNumber(DelegateRegistrationModel delegateRegistrationModel)
        {
            try
            {
                ValidateRegistrationEmail(delegateRegistrationModel);
                return registrationDataService.RegisterNewUserAndDelegateAccount(delegateRegistrationModel);
            }
            catch (DelegateCreationFailedException exception)
            {
                var error = exception.Error;
                var errorMessage = $"Could not create account for delegate on registration. Failure: {error.Name}";

                logger.LogError(exception, errorMessage);

                throw new DelegateCreationFailedException(errorMessage, exception, error);
            }
            catch (Exception exception)
            {
                var error = DelegateCreationError.UnexpectedError;
                var errorMessage = $"Could not create account for delegate on registration. Failure: {error.Name}";

                logger.LogError(exception, errorMessage);

                throw new DelegateCreationFailedException(errorMessage, exception, error);
            }
        }

        private (int delegateId, string candidateNumber) RegisterDelegateAccountAndCentreDetailsForExistingUser(
            int userId,
            DelegateRegistrationModel delegateRegistrationModel
        )
        {
            try
            {
                ValidateCentreEmail(delegateRegistrationModel.CentreSpecificEmail);
                var currentTime = clockService.UtcNow;
                return registrationDataService.RegisterDelegateAccountAndCentreDetailForExistingUser(
                    delegateRegistrationModel,
                    userId,
                    currentTime
                );
            }
            catch (DelegateCreationFailedException exception)
            {
                var error = exception.Error;
                var errorMessage = $"Could not create account for delegate on registration. Failure: {error.Name}";

                logger.LogError(exception, errorMessage);

                throw new DelegateCreationFailedException(errorMessage, exception, error);
            }
            catch (Exception exception)
            {
                var error = DelegateCreationError.UnexpectedError;
                var errorMessage = $"Could not create account for delegate on registration. Failure: {error.Name}";

                logger.LogError(exception, errorMessage);

                throw new DelegateCreationFailedException(errorMessage, exception, error);
            }
        }

        private void ValidateRegistrationEmail(RegistrationModel model)
        {
            var emails =
                (IEnumerable<string>)new[] { model.PrimaryEmail, model.CentreSpecificEmail }.Where(e => e != null);
            if (userDataService.AnyEmailsInSetAreAlreadyInUse(emails))
            {
                var error = DelegateCreationError.EmailAlreadyInUse;
                logger.LogError(
                    $"Could not create account for delegate on registration. Failure: {error.Name}."
                );
                throw new DelegateCreationFailedException(error);
            }
        }

        private void ValidateCentreEmail(string? centreEmail)
        {
            var emails = (IEnumerable<string>)new[] { centreEmail }.Where(e => e != null);
            if (userDataService.AnyEmailsInSetAreAlreadyInUse(emails))
            {
                var error = DelegateCreationError.EmailAlreadyInUse;
                logger.LogError(
                    $"Could not create account for delegate on registration. Failure: {error.Name}."
                );
                throw new DelegateCreationFailedException(error);
            }
        }

        private bool NewDelegateAccountShouldBeApproved(
            string userIp,
            int? supervisorDelegateId,
            IEnumerable<int> supervisorDelegateRecordIdsMatchingDelegate,
            int centreId,
            bool userHasAdminAccountAtCentre = false
        )
        {
            var foundRecordForSupervisorDelegateId = supervisorDelegateId.HasValue &&
                                                     supervisorDelegateRecordIdsMatchingDelegate.Contains(
                                                         supervisorDelegateId.Value
                                                     );

            var centreIpPrefixes = centresDataService.GetCentreIpPrefixes(centreId);
            return userHasAdminAccountAtCentre || foundRecordForSupervisorDelegateId ||
                   centreIpPrefixes.Any(ip => userIp.StartsWith(ip.Trim())) ||
                   userIp == "::1";
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

        private int CreateDelegateAccountForAdmin(AdminRegistrationModel registrationModel)
        {
            var delegateRegistrationModel = new DelegateRegistrationModel(
                registrationModel.FirstName,
                registrationModel.LastName,
                registrationModel.PrimaryEmail,
                registrationModel.CentreSpecificEmail,
                registrationModel.Centre,
                registrationModel.JobGroup,
                registrationModel.PasswordHash!,
                true,
                true,
                registrationModel.ProfessionalRegistrationNumber
            );

            try
            {
                var candidateNumber =
                    registrationDataService.RegisterNewUserAndDelegateAccount(delegateRegistrationModel);
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
                    registrationModel.ProfessionalRegistrationNumber,
                    true
                );

                return userDataService.GetUserIdFromDelegateId(delegateUser.Id);
            }
            catch (Exception exception)
            {
                var error = DelegateCreationError.UnexpectedError;
                var errorMessage = $"Could not create delegate account for admin. Failure: {error.Name}.";

                logger.LogError(exception, errorMessage);

                throw new DelegateCreationFailedException(errorMessage, exception, error);
            }
        }

        private void SendDelegateNeedsApprovalEmailIfNecessary(
            RegistrationModel delegateRegistrationModel,
            bool refactoredTrackingSystemEnabled
        )
        {
            if (delegateRegistrationModel.Approved)
            {
                return;
            }

            var (firstName, _, email) = centresDataService.GetCentreManagerDetails(delegateRegistrationModel.Centre);
            var approvalEmail = GenerateApprovalEmail(
                email,
                firstName,
                delegateRegistrationModel.FirstName,
                delegateRegistrationModel.LastName,
                refactoredTrackingSystemEnabled
            );
            emailService.SendEmail(approvalEmail);
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
