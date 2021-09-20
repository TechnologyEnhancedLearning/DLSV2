namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using Microsoft.Extensions.Configuration;
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

        void RegisterCentreManager(RegistrationModel registrationModel);
    }

    public class RegistrationService : IRegistrationService
    {
        private readonly ICentresDataService centresDataService;
        private readonly IConfiguration config;
        private readonly IEmailService emailService;
        private readonly IPasswordDataService passwordDataService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IRegistrationDataService registrationDataService;
        private readonly ISupervisorDelegateService supervisorDelegateService;
        private readonly IFrameworkNotificationService frameworkNotificationService;

        public RegistrationService(
            IRegistrationDataService registrationDataService,
            IPasswordDataService passwordDataService,
            IPasswordResetService passwordResetService,
            IEmailService emailService,
            ICentresDataService centresDataService,
            IConfiguration config,
            ISupervisorDelegateService supervisorDelegateService,
            IFrameworkNotificationService frameworkNotificationService
        )
        {
            this.registrationDataService = registrationDataService;
            this.passwordDataService = passwordDataService;
            this.passwordResetService = passwordResetService;
            this.emailService = emailService;
            this.centresDataService = centresDataService;
            this.config = config;
            this.supervisorDelegateService = supervisorDelegateService;
            this.frameworkNotificationService = frameworkNotificationService;
        }

        public (string candidateNumber, bool approved) RegisterDelegate(
            DelegateRegistrationModel delegateRegistrationModel,
            string userIp,
            bool refactoredTrackingSystemEnabled,
            int? supervisorDelegateId = null
        )
        {
            var (supervisorDelegateRecord, foundById) = GetSupervisorDelegateRecordByIdOrEmail(
                delegateRegistrationModel.Centre,
                supervisorDelegateId,
                delegateRegistrationModel.Email
            );
            
            var centreIpPrefixes = centresDataService.GetCentreIpPrefixes(delegateRegistrationModel.Centre);
            delegateRegistrationModel.Approved = foundById ||
                                                 centreIpPrefixes.Any(ip => userIp.StartsWith(ip.Trim())) ||
                                                 userIp == "::1";

            var candidateNumber = registrationDataService.RegisterDelegate(delegateRegistrationModel);
            if (candidateNumber == "-1" || candidateNumber == "-4")
            {
                return (candidateNumber, false);
            }

            passwordDataService.SetPasswordByCandidateNumber(candidateNumber, delegateRegistrationModel.PasswordHash!);

            if (supervisorDelegateRecord != null)
            {
                supervisorDelegateService.UpdateSupervisorDelegateRecordStatus(
                    supervisorDelegateRecord.ID,
                    delegateRegistrationModel.Centre,
                    candidateNumber,
                    foundById
                );
                if (foundById)
                {
                    frameworkNotificationService.SendSupervisorDelegateAcceptance(supervisorDelegateRecord.ID);
                }
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

        public void RegisterCentreManager(RegistrationModel registrationModel)
        {
            using var transaction = new TransactionScope();

            CreateDelegateAccountForAdmin(registrationModel);

            registrationDataService.RegisterCentreManagerAdmin(registrationModel);

            centresDataService.SetCentreAutoRegistered(registrationModel.Centre);

            transaction.Complete();
        }

        public string RegisterDelegateByCentre(DelegateRegistrationModel delegateRegistrationModel, string baseUrl)
        {
            var candidateNumber = registrationDataService.RegisterDelegateByCentre(delegateRegistrationModel);
            if (candidateNumber == "-1" || candidateNumber == "-4")
            {
                return candidateNumber;
            }

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
                    baseUrl,
                    delegateRegistrationModel.NotifyDate.Value,
                    "RegisterDelegateByCentre_Refactor"
                );
            }

            return candidateNumber;
        }

        private void CreateDelegateAccountForAdmin(RegistrationModel registrationModel)
        {
            var delegateRegistrationModel = new DelegateRegistrationModel(
                registrationModel.FirstName,
                registrationModel.LastName,
                registrationModel.Email,
                registrationModel.Centre,
                registrationModel.JobGroup,
                registrationModel.PasswordHash!
            ) { Approved = true };
            var candidateNumber = registrationDataService.RegisterDelegate(delegateRegistrationModel);
            if (candidateNumber == "-1" || candidateNumber == "-4")
            {
                throw new Exception(
                    $"Delegate account could not be created (error code: {candidateNumber}) with email address: {registrationModel.Email}"
                );
            }

            passwordDataService.SetPasswordByCandidateNumber(candidateNumber, delegateRegistrationModel.PasswordHash!);
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

            BodyBuilder body = new BodyBuilder
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
                            </body>"
            };

            return new Email(emailSubject, body, emailAddress);
        }

        private (SupervisorDelegate? record, bool foundById) GetSupervisorDelegateRecordByIdOrEmail(
            int centreId,
            int? supervisorDelegateId,
            string email
        )
        {
            SupervisorDelegate? supervisorDelegateRecord = null;
            if (supervisorDelegateId.HasValue)
            {
                supervisorDelegateRecord =
                    supervisorDelegateService.GetPendingSupervisorDelegateRecordByIdAndEmail(
                        centreId,
                        supervisorDelegateId.Value,
                        email
                    );
            }

            var foundById = supervisorDelegateRecord != null;

            supervisorDelegateRecord ??=
                supervisorDelegateService.GetPendingSupervisorDelegateRecordByEmail(centreId, email);

            return (supervisorDelegateRecord, foundById);
        }
    }
}
