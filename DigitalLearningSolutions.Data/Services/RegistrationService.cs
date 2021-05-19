namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Register;
    using MimeKit;

    public interface IRegistrationService
    {
        string RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel, string baseUrl);
    }

    public class RegistrationService: IRegistrationService
    {
        private readonly IRegistrationDataService registrationDataService;
        private readonly IPasswordDataService passwordDataService;
        private readonly IEmailService emailService;
        private readonly ICentresDataService centresDataService;

        public RegistrationService(IRegistrationDataService registrationDataService,
            IPasswordDataService passwordDataService, IEmailService emailService, ICentresDataService centresDataService)
        {
            this.registrationDataService = registrationDataService;
            this.passwordDataService = passwordDataService;
            this.emailService = emailService;
            this.centresDataService = centresDataService;
        }

        public string RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel, string baseUrl)
        {
            var candidateNumber = registrationDataService.RegisterDelegate(delegateRegistrationModel);
            // TODO HEEDLS-446 Handle return string "-4" for duplicate emails
            if (candidateNumber == "-1" || candidateNumber == "-4")
            {
                return candidateNumber;
            }
            passwordDataService.SetPasswordByCandidateNumber(candidateNumber, delegateRegistrationModel.PasswordHash);
            var contactInfo = centresDataService.GetCentreManagerDetails(delegateRegistrationModel.Centre);
            var approvalEmail = GenerateApprovalEmail(contactInfo.email, contactInfo.firstName, delegateRegistrationModel.FirstName,
                delegateRegistrationModel.LastName, baseUrl);
            emailService.SendEmail(approvalEmail);

            return candidateNumber;
        }

        private Email GenerateApprovalEmail(string emailAddress, string firstName, string learnerFirstName, string learnerLastName, string baseUrl)
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
