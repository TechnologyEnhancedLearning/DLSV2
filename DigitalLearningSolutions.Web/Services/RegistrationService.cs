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
        private readonly IGroupsService groupsService;
        private readonly ILogger<RegistrationService> logger;
        private readonly IPasswordDataService passwordDataService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IRegistrationDataService registrationDataService;
        private readonly ISupervisorDelegateService supervisorDelegateService;
        private readonly IUserDataService userDataService;
        private readonly INotificationDataService notificationDataService;
        private readonly IUserService userService;
        private readonly IEmailVerificationDataService emailVerificationDataService;

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
            ILogger<RegistrationService> logger,
            IUserService userService,
            IEmailVerificationDataService emailVerificationDataService,
            IClockUtility clockUtility,
            IGroupsService groupsService
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
            this.userService = userService;
            this.emailVerificationDataService = emailVerificationDataService;
            this.clockUtility = clockUtility;
            this.groupsService = groupsService;
        }

        public (string candidateNumber, bool approved) CreateDelegateAccountForNewUser(
            DelegateRegistrationModel delegateRegistrationModel,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            bool registerJourneyContainsTermsAndConditions,
            int? supervisorDelegateId = null
        )
        {
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
                // TODO: HEEDLS-1014 - Change Delegate ID to User ID
                supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    supervisorDelegateRecordIdsMatchingDelegate,
                    delegateId
                );
            }

            if (!delegateRegistrationModel.Approved)
            {
                SendApprovalEmailToAdmins(delegateRegistrationModel, refactoredTrackingSystemEnabled);
            }

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
                var possibleEmailUpdate = new PossibleEmailUpdate
                {
                    OldEmail = userDataService.GetCentreEmail(userId, internalDelegateRegistrationModel.Centre),
                    NewEmail = delegateRegistrationModel.CentreSpecificEmail,
                    NewEmailIsVerified = emailVerificationDataService.AccountEmailIsVerifiedForUser(
                        userId,
                        delegateRegistrationModel.CentreSpecificEmail
                    ),
                };

                if (delegateAccountAtCentre == null)
                {
                    (delegateId, candidateNumber) =
                        RegisterDelegateAccountAndCentreDetailsForExistingUser(
                            userId,
                            delegateRegistrationModel,
                            possibleEmailUpdate
                        );
                }
                else
                {
                    delegateId = delegateAccountAtCentre.Id;
                    candidateNumber = delegateAccountAtCentre.CandidateNumber;
                    ReregisterDelegateAccountForExistingUser(
                        userId,
                        delegateId,
                        delegateRegistrationModel,
                        possibleEmailUpdate
                    );
                }

                groupsService.AddNewDelegateToAppropriateGroups(delegateId, delegateRegistrationModel);
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
                // TODO: HEEDLS-1014 - Change Delegate ID to User ID
                supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    supervisorDelegateRecordIdsMatchingDelegate,
                    delegateId
                );
            }

            if (!delegateRegistrationModel.Approved)
            {
                SendApprovalEmailToAdmins(delegateRegistrationModel, refactoredTrackingSystemEnabled);
            }

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

            var supervisorDelegateRecordIdsMatchingDelegate =
                GetPendingSupervisorDelegateIdsMatchingDelegate(delegateRegistrationModel).ToList();

            if (delegateRegistrationModel.PasswordHash != null)
            {
                passwordDataService.SetPasswordByCandidateNumber(
                    candidateNumber,
                    delegateRegistrationModel.PasswordHash
                );
            }

            if (delegateRegistrationModel.NotifyDate.HasValue)
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
                // TODO: HEEDLS-1014 - Change Delegate ID to User ID
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
            registrationDataService.RegisterAdmin(
                accountRegistrationModel,
                new PossibleEmailUpdate
                {
                    OldEmail = null,
                    NewEmail = registrationModel.CentreSpecificEmail,
                    NewEmailIsVerified = false,
                }
            );

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

            registrationDataService.RegisterAdmin(
                new AdminAccountRegistrationModel(registrationModel, userId),
                new PossibleEmailUpdate
                {
                    OldEmail = userDataService.GetCentreEmail(userId, centreId),
                    NewEmail = centreSpecificEmail,
                    NewEmailIsVerified = emailVerificationDataService.AccountEmailIsVerifiedForUser(
                        userId,
                        centreSpecificEmail
                    ),
                }
            );
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

                registrationDataService.RegisterAdmin(adminRegistrationModel, null);
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

                groupsService.AddNewDelegateToAppropriateGroups(delegateId, delegateRegistrationModel);

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
            DelegateRegistrationModel delegateRegistrationModel,
            PossibleEmailUpdate emailUpdate
        )
        {
            if (delegateRegistrationModel.CentreSpecificEmail != null)
            {
                ValidateCentreEmail(
                    delegateRegistrationModel.CentreSpecificEmail,
                    delegateRegistrationModel.Centre,
                    userId
                );
            }

            var currentTime = clockUtility.UtcNow;
            return registrationDataService.RegisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                currentTime,
                emailUpdate
            );
        }

        private void ReregisterDelegateAccountForExistingUser(
            int userId,
            int delegateId,
            DelegateRegistrationModel delegateRegistrationModel,
            PossibleEmailUpdate emailUpdate
        )
        {
            if (delegateRegistrationModel.CentreSpecificEmail != null)
            {
                ValidateCentreEmail(
                    delegateRegistrationModel.CentreSpecificEmail,
                    delegateRegistrationModel.Centre,
                    userId
                );
            }

            var currentTime = clockUtility.UtcNow;
            registrationDataService.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                delegateId,
                currentTime,
                emailUpdate
            );
        }

        private void ValidateCentreEmail(string centreEmail, int centreId, int? idOfRegistrantIfAlreadyExisting)
        {
            var centreEmailIsInUse = idOfRegistrantIfAlreadyExisting == null
                ? userDataService.CentreSpecificEmailIsInUseAtCentre(centreEmail, centreId)
                : userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    centreEmail,
                    centreId,
                    idOfRegistrantIfAlreadyExisting.Value
                );

            if (centreEmailIsInUse)
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
            var delegateEmails = new List<string?>
                { delegateRegistrationModel.PrimaryEmail, delegateRegistrationModel.CentreSpecificEmail };

            return supervisorDelegateService
                .GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
                    delegateRegistrationModel.Centre,
                    delegateEmails
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

        private void SendApprovalEmailToAdmins(
            RegistrationModel delegateRegistrationModel,
            bool refactoredTrackingSystemEnabled
        )
        {
            var recipients = notificationDataService.GetAdminRecipientsForCentreNotification(
                delegateRegistrationModel.Centre,
                4 // NotificationId 4 is "Delegate registration requires approval"
            );

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
