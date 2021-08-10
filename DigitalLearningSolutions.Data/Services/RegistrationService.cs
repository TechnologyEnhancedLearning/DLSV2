namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Configuration;
    using MimeKit;

    public interface IRegistrationService
    {
        (string candidateNumber, bool approved) RegisterDelegate(
            DelegateRegistrationModel delegateRegistrationModel,
            string userIp,
            bool refactoredTrackingSystemEnabled
        );

        string RegisterDelegateByCentre(DelegateRegistrationModel delegateRegistrationModel);

        void RegisterCentreManager(RegistrationModel registrationModel);

        void GenerateAndSendDelegateWelcomeEmail(DelegateUserCard delegateUser);
    }

    public class RegistrationService : IRegistrationService
    {
        private readonly ICentresDataService centresDataService;
        private readonly IConfiguration config;
        private readonly IEmailService emailService;
        private readonly IPasswordDataService passwordDataService;
        private readonly IRegistrationDataService registrationDataService;

        public RegistrationService(
            IRegistrationDataService registrationDataService,
            IPasswordDataService passwordDataService,
            IEmailService emailService,
            ICentresDataService centresDataService,
            IConfiguration config
        )
        {
            this.registrationDataService = registrationDataService;
            this.passwordDataService = passwordDataService;
            this.emailService = emailService;
            this.centresDataService = centresDataService;
            this.config = config;
        }

        public (string candidateNumber, bool approved) RegisterDelegate(
            DelegateRegistrationModel delegateRegistrationModel,
            string userIp,
            bool refactoredTrackingSystemEnabled
        )
        {
            var centreIpPrefixes =
                centresDataService.GetCentreIpPrefixes(delegateRegistrationModel.Centre);
            delegateRegistrationModel.Approved =
                centreIpPrefixes.Any(ip => userIp.StartsWith(ip.Trim())) || userIp == "::1";

            var candidateNumber =
                registrationDataService.RegisterDelegate(delegateRegistrationModel);
            if (candidateNumber == "-1" || candidateNumber == "-4")
            {
                return (candidateNumber, false);
            }

            passwordDataService.SetPasswordByCandidateNumber(
                candidateNumber,
                delegateRegistrationModel.PasswordHash!
            );
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

        public string RegisterDelegateByCentre(DelegateRegistrationModel delegateRegistrationModel)
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

            return candidateNumber;
        }

        public void RegisterCentreManager(RegistrationModel registrationModel)
        {
            using var transaction = new TransactionScope();

            CreateDelegateAccountForAdmin(registrationModel);

            registrationDataService.RegisterCentreManagerAdmin(registrationModel);

            centresDataService.SetCentreAutoRegistered(registrationModel.Centre);

            transaction.Complete();
        }

        public void GenerateAndSendDelegateWelcomeEmail(DelegateUserCard delegateUser)
        {
            using var transaction = new TransactionScope();

            var resetPasswordEmail = GenerateWelcomeEmail(
                delegateUser.EmailAddress!,
                delegateUser.FirstName!,
                delegateUser.LastName,
                delegateUser.CentreName,
                delegateUser.CandidateNumber
            );
            emailService.SendEmail(resetPasswordEmail);
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
            var candidateNumber =
                registrationDataService.RegisterDelegate(delegateRegistrationModel);
            if (candidateNumber == "-1" || candidateNumber == "-4")
            {
                throw new Exception(
                    $"Delegate account could not be created (error code: {candidateNumber}) with email address: {registrationModel.Email}"
                );
            }

            passwordDataService.SetPasswordByCandidateNumber(
                candidateNumber,
                delegateRegistrationModel.PasswordHash!
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

            BodyBuilder body = new BodyBuilder
            {
                TextBody = $@"Dear {firstName},
                            A learner, {learnerFirstName} {learnerLastName}, has registered against your Digital Learning Solutions centre and requires approval before they can access courses.
                            To approve or reject their registration please follow this link: {approvalUrl}
                            Please don't reply to this email as it has been automatically generated.",
                HtmlBody = $@"<body style= 'font - family: Calibri; font - size: small;'>
                                <p>Dear {firstName},</p>
                                <p>A learner, {learnerFirstName} {learnerLastName}, has registered against your Digital Learning Solutions centre and requires approval before they can access courses.</p>
                                <p>To approve or reject their registration please follow this link: <a href=""{approvalUrl}"">{approvalUrl}</a></p>
                                <p>Please don't reply to this email as it has been automatically generated.</p>
                            </body>"
            };

            return new Email(emailSubject, body, emailAddress);
        }

        private Email GenerateWelcomeEmail(
            string emailAddress,
            string firstName,
            string lastName,
            string centreName,
            string candidateNumber
        )
        {
            const string emailSubject = "Welcome to Digital Learning Solutions - Verify your Registration";
            var setPasswordUrl = $"{config["AppRootPath"]}"; // TODO

            BodyBuilder body = new BodyBuilder
            {
                TextBody = $@"Dear {firstName} {lastName},
                            An administrator has registered your details to give you access to the Digital Learning Solutions (DLS) platform under the centre {centreName}.
                            You have been assigned the unique DLS delegate number {candidateNumber}.
                            To complete your registration and access your Digital Learning Solutions content, please follow this link:{setPasswordUrl}
                            Note that this link can only be used once.
                            Please don't reply to this email as it has been automatically generated.",
                HtmlBody = $@"<body style= 'font - family: Calibri; font - size: small;'>
                                <p>Dear {firstName},</p>
                                <p>An administrator has registered your details to give you access to the Digital Learning Solutions (DLS) platform under the centre {centreName}.</p>
                                <p>You have been assigned the unique DLS delegate number {candidateNumber}.</p>
                                <p>To complete your registration and access your Digital Learning Solutions content, please follow this link: <a href=""{setPasswordUrl}"">{setPasswordUrl}</a></p>
                                <p>Note that this link can only be used once.</p>
                                <p>Please don't reply to this email as it has been automatically generated.</p>
                            </body>"
            };

            return new Email(emailSubject, body, emailAddress);
        }
    }
}
