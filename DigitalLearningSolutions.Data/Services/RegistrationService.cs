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
            bool registerJourneyContainsTermsAndConditions,
            int? inviteId = null
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

        void PromoteDelegateToAdmin(AdminRoles adminRoles, int? categoryId, int delegateId);

        string CreateAccountAndReturnCandidateNumber(
            DelegateRegistrationModel delegateRegistrationModel,
            bool registerJourneyContainsTermsAndConditions
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

        public RegistrationService(
            IRegistrationDataService registrationDataService,
            IPasswordDataService passwordDataService,
            IPasswordResetService passwordResetService,
            IEmailService emailService,
            ICentresDataService centresDataService,
            IConfiguration config,
            ISupervisorDelegateService supervisorDelegateService,
            IUserDataService userDataService,
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
            this.logger = logger;
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

            var foundRecordForSupervisorDelegateId = supervisorDelegateId.HasValue &&
                                                     supervisorDelegateRecordIdsMatchingDelegate.Contains(
                                                         supervisorDelegateId.Value
                                                     );

            var centreIpPrefixes = centresDataService.GetCentreIpPrefixes(delegateRegistrationModel.Centre);
            delegateRegistrationModel.Approved = foundRecordForSupervisorDelegateId ||
                                                 centreIpPrefixes.Any(ip => userIp.StartsWith(ip.Trim())) ||
                                                 userIp == "::1";

            var candidateNumber = CreateAccountAndReturnCandidateNumber(
                delegateRegistrationModel,
                registerJourneyContainsTermsAndConditions
            );

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

        public string RegisterDelegateByCentre(
            DelegateRegistrationModel delegateRegistrationModel,
            string baseUrl,
            bool registerJourneyContainsTermsAndConditions
        )
        {
            using var transaction = new TransactionScope();

            var candidateNumber = CreateAccountAndReturnCandidateNumber(
                delegateRegistrationModel,
                registerJourneyContainsTermsAndConditions
            );

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

        public void RegisterCentreManager(
            AdminRegistrationModel registrationModel,
            bool registerJourneyContainsTermsAndConditions
        )
        {
            using var transaction = new TransactionScope();

            var userId = CreateDelegateAccountForAdmin(registrationModel, registerJourneyContainsTermsAndConditions);

            registrationDataService.RegisterAdmin(registrationModel, userId);

            centresDataService.SetCentreAutoRegistered(registrationModel.Centre);

            transaction.Complete();
        }

        public void PromoteDelegateToAdmin(AdminRoles adminRoles, int? categoryId, int delegateId)
        {
            var delegateUser = userDataService.GetDelegateUserById(delegateId)!;

            var userId = userDataService.GetUserIdFromDelegateId(delegateId);

            var admin = userDataService.GetAdminAccountsByUserId(userId)
                .SingleOrDefault(a => a.CentreId == delegateUser.CentreId);

            if (admin != null)
            {
                if (admin.Active)
                {
                    throw new AdminCreationFailedException(AdminCreationError.ActiveAdminAlreadyExists);
                }

                userDataService.ReactivateAdmin(admin.Id);
                userDataService.UpdateAdminUserPermissions(
                    admin.Id,
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
                var adminRegistrationModel = new AdminRegistrationModel(
                    delegateUser.CentreId,
                    true,
                    true,
                    categoryId,
                    adminRoles.IsCentreAdmin,
                    false,
                    adminRoles.IsSupervisor,
                    adminRoles.IsNominatedSupervisor,
                    adminRoles.IsTrainer,
                    adminRoles.IsContentCreator,
                    adminRoles.IsCmsAdministrator,
                    adminRoles.IsCmsManager,
                    delegateUser.JobGroupId
                );

                registrationDataService.RegisterAdmin(adminRegistrationModel, userId);
            }
        }

        public string CreateAccountAndReturnCandidateNumber(
            DelegateRegistrationModel delegateRegistrationModel,
            bool registerJourneyContainsTermsAndConditions
        )
        {
            try
            {
                ValidateRegistrationEmail(delegateRegistrationModel);
                return registrationDataService.RegisterNewUserAndDelegateAccount(
                    delegateRegistrationModel,
                    registerJourneyContainsTermsAndConditions
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

        private void ValidateRegistrationEmail(DelegateRegistrationModel model)
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
                registrationModel.ProfessionalRegistrationNumber
            );

            try
            {
                var candidateNumber =
                    registrationDataService.RegisterNewUserAndDelegateAccount(
                        delegateRegistrationModel,
                        registerJourneyContainsTermsAndConditions
                    );
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
