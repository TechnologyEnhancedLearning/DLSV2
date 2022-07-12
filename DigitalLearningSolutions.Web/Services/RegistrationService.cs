namespace DigitalLearningSolutions.Web.Services
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
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    public interface IRegistrationService
    {
        (string candidateNumber, bool approved) CreateDelegateAccountForNewUser(
            DelegateRegistrationModel delegateRegistrationModel,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            bool registerJourneyContainsTermsAndConditions,
            int? inviteId = null
        );

        (string candidateNumber, bool approved, bool userHasAdminAccountAtCentre) CreateDelegateAccountForExistingUser(
            InternalDelegateRegistrationModel internalDelegateRegistrationModel,
            int userId,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            int? supervisorDelegateId = null
        );

        string RegisterDelegateByCentre(
            DelegateRegistrationModel delegateRegistrationModel,
            string baseUrl,
            bool registerJourneyContainsTermsAndConditions
        );

        void RegisterCentreManager(
            AdminRegistrationModel registrationModel,
            bool registerJourneyContainsTermsAndConditions
        );

        void CreateCentreManagerForExistingUser(int userId, int centreId, string? centreSpecificEmail);

        void PromoteDelegateToAdmin(AdminRoles adminRoles, int? categoryId, int userId, int centreId);

        (int delegateId, string candidateNumber) CreateAccountAndReturnCandidateNumberAndDelegateId(
            DelegateRegistrationModel delegateRegistrationModel,
            bool registerJourneyContainsTermsAndConditions
        );
    }

    public class RegistrationService : IRegistrationService
    {
        private readonly ICentresDataService centresDataService;
        private readonly IClockUtility clockUtility;
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
            IClockUtility clockUtility
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
            this.clockUtility = clockUtility;
        }

        public (string candidateNumber, bool approved) CreateDelegateAccountForNewUser(
            DelegateRegistrationModel delegateRegistrationModel,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            bool registerJourneyContainsTermsAndConditions,
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

            var (delegateId, candidateNumber) = CreateAccountAndReturnCandidateNumberAndDelegateId(
                delegateRegistrationModel,
                registerJourneyContainsTermsAndConditions
            );

            passwordDataService.SetPasswordByCandidateNumber(
                candidateNumber,
                delegateRegistrationModel.PasswordHash!
            );
            userDataService.UpdateDelegateProfessionalRegistrationNumber(
                delegateId,
                delegateRegistrationModel.ProfessionalRegistrationNumber,
                true
            );

            if (supervisorDelegateRecordIdsMatchingDelegate.Any())
            {
                supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    supervisorDelegateRecordIdsMatchingDelegate,
                    delegateId
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

            var userAccountsAtCentre = userEntity.GetCentreAccountSet(internalDelegateRegistrationModel.Centre);
            var userHasAdminAccountAtCentre = userAccountsAtCentre?.CanLogIntoAdminAccount == true;

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

            var delegateAccountAtCentre = userAccountsAtCentre?.DelegateAccount;

            if (delegateAccountAtCentre?.Active == true)
            {
                var errorMessage =
                    "Could not create account for delegate on registration. " +
                    $"Failure: active delegate account with ID {delegateAccountAtCentre.Id} already exists " +
                    $"at centre with ID {delegateAccountAtCentre.CentreId} for user with ID {delegateAccountAtCentre.UserId}";
                throw new DelegateCreationFailedException(
                    errorMessage,
                    DelegateCreationError.ActiveAccountAlreadyExists
                );
            }

            int delegateId;
            string candidateNumber;

            try
            {
                if (delegateAccountAtCentre == null)
                {
                    (delegateId, candidateNumber) =
                        RegisterDelegateAccountAndCentreDetailsForExistingUser(userId, delegateRegistrationModel);
                }
                else
                {
                    delegateId = delegateAccountAtCentre.Id;
                    candidateNumber = delegateAccountAtCentre.CandidateNumber;
                    ReregisterDelegateAccountForExistingUser(userId, delegateId, delegateRegistrationModel);
                }
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

        public string RegisterDelegateByCentre(
            DelegateRegistrationModel delegateRegistrationModel,
            string baseUrl,
            bool registerJourneyContainsTermsAndConditions
        )
        {
            using var transaction = new TransactionScope();

            var (delegateId, candidateNumber) = CreateAccountAndReturnCandidateNumberAndDelegateId(
                delegateRegistrationModel,
                registerJourneyContainsTermsAndConditions
            );

            // TODO HEEDLS-899 sort out supervisor delegate stuff
            var supervisorDelegateRecordIdsMatchingDelegate =
                GetPendingSupervisorDelegateIdsMatchingDelegate(delegateRegistrationModel).ToList();

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
                    delegateId,
                    baseUrl,
                    delegateRegistrationModel.NotifyDate.Value,
                    "RegisterDelegateByCentre_Refactor"
                );
            }

            userDataService.UpdateDelegateProfessionalRegistrationNumber(
                delegateId,
                delegateRegistrationModel.ProfessionalRegistrationNumber,
                true
            );

            if (supervisorDelegateRecordIdsMatchingDelegate.Any())
            {
                supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    supervisorDelegateRecordIdsMatchingDelegate,
                    delegateId
                );
            }

            transaction.Complete();

            return candidateNumber;
        }

        public void RegisterCentreManager(
            AdminRegistrationModel registrationModel,
            bool registerJourneyContainsTermsAndConditions
        )
        {
            using var transaction = new TransactionScope();

            var userId = CreateDelegateAccountForAdmin(registrationModel, registerJourneyContainsTermsAndConditions);

            var accountRegistrationModel = new AdminAccountRegistrationModel(registrationModel, userId);
            registrationDataService.RegisterAdmin(accountRegistrationModel);

            centresDataService.SetCentreAutoRegistered(registrationModel.Centre);

            transaction.Complete();
        }

        public void CreateCentreManagerForExistingUser(int userId, int centreId, string? centreSpecificEmail)
        {
            using var transaction = new TransactionScope();

            var userAccount = userDataService.GetUserAccountById(userId)!;
            var registrationModel = new AdminRegistrationModel(
                userAccount.FirstName,
                userAccount.LastName,
                userAccount.PrimaryEmail,
                centreSpecificEmail,
                centreId,
                userAccount.PasswordHash,
                true,
                true,
                userAccount.ProfessionalRegistrationNumber,
                userAccount.JobGroupId,
                null,
                true,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                userAccount.ProfileImage
            );

            registrationDataService.RegisterAdmin(new AdminAccountRegistrationModel(registrationModel, userId));
            centresDataService.SetCentreAutoRegistered(registrationModel.Centre);

            transaction.Complete();
        }

        public void PromoteDelegateToAdmin(AdminRoles adminRoles, int? categoryId, int userId, int centreId)
        {
            var adminAtCentre = userDataService.GetAdminAccountsByUserId(userId)
                .SingleOrDefault(a => a.CentreId == centreId);

            if (adminAtCentre != null)
            {
                if (adminAtCentre.Active)
                {
                    throw new AdminCreationFailedException("Active admin already exists for this user at this centre");
                }

                userDataService.ReactivateAdmin(adminAtCentre.Id);
                userDataService.UpdateAdminUserPermissions(
                    adminAtCentre.Id,
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
            else
            {
                var adminRegistrationModel = new AdminAccountRegistrationModel(
                    userId,
                    null,
                    centreId,
                    categoryId,
                    adminRoles.IsCentreAdmin,
                    false,
                    adminRoles.IsContentManager,
                    adminRoles.IsContentCreator,
                    adminRoles.IsTrainer,
                    adminRoles.ImportOnly,
                    adminRoles.IsSupervisor,
                    adminRoles.IsNominatedSupervisor,
                    true
                );

                registrationDataService.RegisterAdmin(adminRegistrationModel);
            }
        }

        public (int delegateId, string candidateNumber) CreateAccountAndReturnCandidateNumberAndDelegateId(
            DelegateRegistrationModel delegateRegistrationModel,
            bool registerJourneyContainsTermsAndConditions
        )
        {
            try
            {
                var primaryEmailIsInvalid = userDataService.PrimaryEmailIsInUse(delegateRegistrationModel.PrimaryEmail);
                var centreSpecificEmailIsInvalid =
                    delegateRegistrationModel.CentreSpecificEmail != null &&
                    userDataService.CentreSpecificEmailIsInUseAtCentre(
                        delegateRegistrationModel.CentreSpecificEmail,
                        delegateRegistrationModel.Centre
                    );

                if (primaryEmailIsInvalid || centreSpecificEmailIsInvalid)
                {
                    throw new DelegateCreationFailedException(DelegateCreationError.EmailAlreadyInUse);
                }

                var (delegateId, candidateNumber) = registrationDataService.RegisterNewUserAndDelegateAccount(
                    delegateRegistrationModel,
                    registerJourneyContainsTermsAndConditions
                );

                return (delegateId, candidateNumber);
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
            if (delegateRegistrationModel.CentreSpecificEmail != null)
            {
                ValidateCentreEmail(delegateRegistrationModel.CentreSpecificEmail, delegateRegistrationModel.Centre);
            }

            var currentTime = clockUtility.UtcNow;
            return registrationDataService.RegisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                currentTime
            );
        }

        private void ReregisterDelegateAccountForExistingUser(
            int userId,
            int delegateId,
            DelegateRegistrationModel delegateRegistrationModel
        )
        {
            if (delegateRegistrationModel.CentreSpecificEmail != null)
            {
                ValidateCentreEmail(delegateRegistrationModel.CentreSpecificEmail, delegateRegistrationModel.Centre);
            }

            var currentTime = clockUtility.UtcNow;
            registrationDataService.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                delegateId,
                currentTime
            );
        }

        private void ValidateCentreEmail(string centreEmail, int centreId)
        {
            if (userDataService.CentreSpecificEmailIsInUseAtCentre(centreEmail, centreId))
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

        private int CreateDelegateAccountForAdmin(
            AdminRegistrationModel registrationModel,
            bool registerJourneyContainsTermsAndConditions
        )
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
                true,
                registrationModel.ProfessionalRegistrationNumber
            );

            try
            {
                var (delegateId, candidateNumber) = registrationDataService.RegisterNewUserAndDelegateAccount(
                    delegateRegistrationModel,
                    registerJourneyContainsTermsAndConditions
                );

                passwordDataService.SetPasswordByCandidateNumber(
                    candidateNumber,
                    delegateRegistrationModel.PasswordHash!
                );

                userDataService.UpdateDelegateProfessionalRegistrationNumber(
                    delegateId,
                    registrationModel.ProfessionalRegistrationNumber,
                    true
                );

                return userDataService.GetUserIdFromDelegateId(delegateId);
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
