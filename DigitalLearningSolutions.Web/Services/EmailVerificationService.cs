namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using MimeKit;

    public interface IEmailVerificationService
    {
        bool AccountEmailIsVerifiedForUser(int userId, string email);

        void CreateEmailVerificationHashesAndSendVerificationEmails(
            UserAccount userAccount,
            List<string> unverifiedEmails,
            string baseUrl
        );
    }

    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IClockUtility clockUtility;
        private readonly IEmailService emailService;
        private readonly IEmailVerificationDataService emailVerificationDataService;

        public EmailVerificationService(
            IEmailVerificationDataService emailVerificationDataService,
            IEmailService emailService,
            IClockUtility clockUtility
        )
        {
            this.emailVerificationDataService = emailVerificationDataService;
            this.emailService = emailService;
            this.clockUtility = clockUtility;
        }

        public bool AccountEmailIsVerifiedForUser(int userId, string email)
        {
            return emailVerificationDataService.AccountEmailIsVerifiedForUser(userId, email);
        }

        public void CreateEmailVerificationHashesAndSendVerificationEmails(
            UserAccount userAccount,
            List<string> unverifiedEmails,
            string baseUrl
        )
        {
            foreach (var email in unverifiedEmails.Distinct())
            {
                var hash = Guid.NewGuid().ToString();
                var hashId = emailVerificationDataService.CreateEmailVerificationHash(hash, clockUtility.UtcNow);
                var emailAddress = email!;

                UpdateEmailVerificationHashId(userAccount.Id, emailAddress, hashId);

                emailService.SendEmail(
                    GenerateVerificationEmail(userAccount, hash, emailAddress, baseUrl)
                );
            }
        }

        private void UpdateEmailVerificationHashId(int userId, string? emailAddress, int hashId)
        {
            emailVerificationDataService.UpdateEmailVerificationHashIdForPrimaryEmail(userId, emailAddress, hashId);
            emailVerificationDataService.UpdateEmailVerificationHashIdForCentreEmails(userId, emailAddress, hashId);
        }

        private static Email GenerateVerificationEmail(
            UserAccount userAccount,
            string emailVerificationHash,
            string emailAddress,
            string baseUrl
        )
        {
            var verifyEmailUrl = new UriBuilder(baseUrl);

            if (!verifyEmailUrl.Path.EndsWith('/'))
            {
                verifyEmailUrl.Path += '/';
            }

            verifyEmailUrl.Path += "VerifyEmail";
            verifyEmailUrl.Query = $"code={emailVerificationHash}&email={emailAddress}";
            const string emailSubject = "Digital Learning Solutions - Verify your email address";

            var body = new BodyBuilder
            {
                TextBody = $@"Dear {userAccount.FullName},
                            Please click the following link to verify your email address for Digital Learning Solutions: {verifyEmailUrl.Uri}",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear {userAccount.FullName},</p>
                                <p>Please <a href=""{verifyEmailUrl.Uri}"">click here to verify your email address for Digital Learning Solutions</a></p>
                            </body>",
            };

            return new Email(emailSubject, body, emailAddress);
        }
    }
}
