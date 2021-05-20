﻿namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Register;
    using MimeKit;

    public interface IRegistrationService
    {
        (string candidateNumber, bool approved) RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel,
            string baseUrl, string userIp);
    }

    public class RegistrationService : IRegistrationService
    {
        private readonly ICentresDataService centresDataService;
        private readonly IEmailService emailService;
        private readonly IPasswordDataService passwordDataService;
        private readonly IRegistrationDataService registrationDataService;

        public RegistrationService
        (
            IRegistrationDataService registrationDataService,
            IPasswordDataService passwordDataService,
            IEmailService emailService,
            ICentresDataService centresDataService
        )
        {
            this.registrationDataService = registrationDataService;
            this.passwordDataService = passwordDataService;
            this.emailService = emailService;
            this.centresDataService = centresDataService;
        }

        public (string candidateNumber, bool approved) RegisterDelegate
        (
            DelegateRegistrationModel delegateRegistrationModel,
            string baseUrl,
            string userIP
        )
        {
            var centreIPPrefixes = centresDataService.GetCentreIPPrefix(delegateRegistrationModel.Centre);
            delegateRegistrationModel.Approved =
                centreIPPrefixes.Any(ip => userIP.StartsWith(ip.Trim())) || userIP == "::1";

            var candidateNumber = registrationDataService.RegisterDelegate(delegateRegistrationModel);
            // TODO HEEDLS-446 Handle return string "-4" for duplicate emails
            if (candidateNumber == "-1")
            {
                return (candidateNumber, false);
            }

            passwordDataService.SetPasswordByCandidateNumber(candidateNumber, delegateRegistrationModel.PasswordHash);

            if (!delegateRegistrationModel.Approved)
            {
                var contactInfo = centresDataService.GetCentreManagerDetails(delegateRegistrationModel.Centre);
                var approvalEmail = GenerateApprovalEmail(contactInfo.email, contactInfo.firstName,
                    delegateRegistrationModel.FirstName,
                    delegateRegistrationModel.LastName, baseUrl);
                emailService.SendEmail(approvalEmail);
            }

            return (candidateNumber, delegateRegistrationModel.Approved);
        }

        private Email GenerateApprovalEmail
        (
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

            approvalUrl.Path += "tracking/approvedelegates";

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
