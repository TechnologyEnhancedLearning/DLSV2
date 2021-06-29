namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Register;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    public interface IRegistrationService
    {
        (string candidateNumber, bool approved) RegisterDelegate
        (
            DelegateRegistrationModel delegateRegistrationModel,
            string baseUrl,
            string userIp
        );

        string RegisterAdminDelegate(
            RegistrationModel registrationModel
        );

        bool RegisterCentreManager(RegistrationModel registrationModel);
    }

    public class RegistrationService : IRegistrationService
    {
        private readonly ICentresDataService centresDataService;
        private readonly IEmailService emailService;
        private readonly IPasswordDataService passwordDataService;
        private readonly IRegistrationDataService registrationDataService;
        private readonly ILogger<RegistrationService> logger;

        public RegistrationService
        (
            IRegistrationDataService registrationDataService,
            IPasswordDataService passwordDataService,
            IEmailService emailService,
            ICentresDataService centresDataService,
            ILogger<RegistrationService> logger
        )
        {
            this.registrationDataService = registrationDataService;
            this.passwordDataService = passwordDataService;
            this.emailService = emailService;
            this.centresDataService = centresDataService;
            this.logger = logger;
        }

        public (string candidateNumber, bool approved) RegisterDelegate
        (
            DelegateRegistrationModel delegateRegistrationModel,
            string baseUrl,
            string userIp
        )
        {
            var centreIpPrefixes = centresDataService.GetCentreIpPrefixes(delegateRegistrationModel.Centre);
            delegateRegistrationModel.Approved =
                centreIpPrefixes.Any(ip => userIp.StartsWith(ip.Trim())) || userIp == "::1";

            var candidateNumber = registrationDataService.RegisterDelegate(delegateRegistrationModel);
            if (candidateNumber == "-1" || candidateNumber == "-4")
            {
                return (candidateNumber, false);
            }

            passwordDataService.SetPasswordByCandidateNumber(candidateNumber, delegateRegistrationModel.PasswordHash);

            if (!delegateRegistrationModel.Approved)
            {
                var contactInfo = centresDataService.GetCentreManagerDetails(delegateRegistrationModel.Centre);
                var approvalEmail = GenerateApprovalEmail(
                    contactInfo.email,
                    contactInfo.firstName,
                    delegateRegistrationModel.FirstName,
                    delegateRegistrationModel.LastName,
                    baseUrl
                );
                emailService.SendEmail(approvalEmail);
            }

            return (candidateNumber, delegateRegistrationModel.Approved);
        }

        public string RegisterAdminDelegate(
            RegistrationModel registrationModel
        )
        {
            var delegateRegistrationModel = new DelegateRegistrationModel(
                registrationModel.FirstName,
                registrationModel.LastName,
                registrationModel.Email,
                registrationModel.Centre,
                registrationModel.JobGroup,
                registrationModel.PasswordHash,
                null,
                null,
                null,
                null,
                null,
                null
            );
            delegateRegistrationModel.Approved = true;
            var candidateNumber = registrationDataService.RegisterDelegate(delegateRegistrationModel);
            if (candidateNumber == "-1" || candidateNumber == "-4")
            {
                return candidateNumber;
            }

            passwordDataService.SetPasswordByCandidateNumber(candidateNumber, delegateRegistrationModel.PasswordHash);

            return candidateNumber;
        }

        public bool RegisterCentreManager(
            RegistrationModel registrationModel
        )
        {
            using var transaction = new TransactionScope();
            try
            {
                var candidateNumber = RegisterAdminDelegate(registrationModel);
                if (candidateNumber == "-1")
                {
                    throw new Exception("Delegate account could not be created");
                }

                registrationDataService.RegisterCentreManagerAdmin(registrationModel);

                centresDataService.SetCentreAutoRegistered(registrationModel.Centre);

                transaction.Complete();

                return true;
            }
            catch (Exception e)
            {
                logger.LogWarning($"Centre Manager registration failed for the following reason: {e.Message}");
                return false;
            }
        }

        private Email GenerateApprovalEmail(
            string emailAddress,
            string firstName,
            string learnerFirstName,
            string learnerLastName,
            string baseUrl
        )
        {
            UriBuilder approvalUrl = new UriBuilder(baseUrl);
            if (!approvalUrl.Path.EndsWith('/'))
            {
                approvalUrl.Path += '/';
            }

            approvalUrl.Path += "TrackingSystem/Delegates/Approve";

            string emailSubject = "Digital Learning Solutions Registration Requires Approval";

            BodyBuilder body = new BodyBuilder
            {
                TextBody = $@"Dear {firstName},
                            A learner, {learnerFirstName} {learnerLastName}, has registered against your Digital Learning Solutions centre and requires approval before they can access courses.
                            To approve or reject their registration please follow this link: {approvalUrl.Uri}
                            Please don't reply to this email as it has been automatically generated.",
                HtmlBody = $@"<body style= 'font - family: Calibri; font - size: small;'>
                                <p>Dear {firstName},</p>
                                <p>A learner, {learnerFirstName} {learnerLastName}, has registered against your Digital Learning Solutions centre and requires approval before they can access courses.</p>
                                <p>To approve or reject their registration please follow this link: <a href=""{approvalUrl.Uri}"">{approvalUrl.Uri}</a></p>
                                <p>Please don't reply to this email as it has been automatically generated.</p>
                            </body>"
            };

            return new Email(emailSubject, body, emailAddress);
        }
    }
}
