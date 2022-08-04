namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Configuration;
    using MimeKit;

    public interface IEmailVerificationService
    {
        void SendVerificationEmails(UserAccount userAccount, IEnumerable<(string, int?)> emails);
    }

    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IEmailVerificationDataService emailVerificationDataService;
        private readonly IEmailService emailService;
        private readonly IClockUtility clockUtility;
        private readonly IConfiguration config;

        public EmailVerificationService(
            IEmailVerificationDataService emailVerificationDataService,
            IEmailService emailService,
            IClockUtility clockUtility,
            IConfiguration config
        )
        {
            this.emailVerificationDataService = emailVerificationDataService;
            this.emailService = emailService;
            this.clockUtility = clockUtility;
            this.config = config;
        }

        public void SendVerificationEmails(UserAccount userAccount, IEnumerable<(string, int?)> emails)
        {
            var unverifiedEmails = emails.Where(
                emailCentrePair =>
                    !emailVerificationDataService.IsEmailVerifiedForUser(userAccount.Id, emailCentrePair.Item1)
            ).ToList();

            foreach (var emailGroup in unverifiedEmails.GroupBy(emailCentrePair => emailCentrePair.Item1))
            {
                var hash = Guid.NewGuid().ToString();
                var hashId = emailVerificationDataService.CreateEmailVerificationHash(hash, clockUtility.UtcNow);

                foreach (var (_, centreId) in emailGroup)
                {
                    UpdateEmailVerificationHashId(userAccount.Id, centreId, hashId);
                }

                emailService.SendEmail(
                    GenerateVerificationEmail(userAccount, hash, emailGroup.Key, config.GetAppRootPath())
                );
            }
        }

        private void UpdateEmailVerificationHashId(int userId, int? centreId, int hashId)
        {
            if (centreId == null)
            {
                emailVerificationDataService.UpdateEmailVerificationHashIdForPrimaryEmail(userId, hashId);
            }
            else
            {
                emailVerificationDataService.UpdateEmailVerificationHashIdForCentreEmail(
                    userId,
                    centreId.Value,
                    hashId
                );
            }
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

            verifyEmailUrl.Path += "VerifyYourEmail";
            verifyEmailUrl.Query = $"code={emailVerificationHash}&email={emailAddress}";
            const string emailSubject = "Digital Learning Solutions - Verify your email address";

            var body = new BodyBuilder
            {
                TextBody = $@"Dear {userAccount.FullName},
                            Please click the following link to verify your email address for Digital Learning Solutions: {verifyEmailUrl.Uri}",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear {userAccount.FullName},</p>
                                <p>Please click the following link to verify your email address for Digital Learning Solutions: <a href=""{verifyEmailUrl.Uri}""></a></p>
                            </body>",
            };

            return new Email(emailSubject, body, emailAddress);
        }
    }
}
