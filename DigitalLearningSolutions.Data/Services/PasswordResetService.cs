namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Auth;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    public interface IPasswordResetService
    {
        Task<bool> EmailAndResetPasswordHashAreValidAsync(string emailAddress, string resetHash);
        void GenerateAndSendPasswordResetLink(string emailAddress, string baseUrl);
        Task InvalidateResetPasswordForEmailAsync(string email);
        Task ChangePasswordAsync(string email, string newPassword);
    }

    public class PasswordResetService : IPasswordResetService
    {
        private readonly IPasswordResetDataService passwordResetDataService;
        private readonly IEmailService emailService;
        private readonly IClockService clockService;
        private readonly ICryptoService cryptoService;
        private readonly IPasswordDataService passwordDataService;
        private readonly ILogger<PasswordResetService> logger;

        private readonly IUserService userService;

        public PasswordResetService(
            IUserService userService,
            IPasswordResetDataService passwordResetDataService,
            ILogger<PasswordResetService> logger,
            IEmailService emailService,
            IClockService clockService,
            ICryptoService cryptoService,
            IPasswordDataService passwordDataService)
        {
            this.userService = userService;
            this.passwordResetDataService = passwordResetDataService;
            this.logger = logger;
            this.emailService = emailService;
            this.clockService = clockService;
            this.cryptoService = cryptoService;
            this.passwordDataService = passwordDataService;
        }

        public async Task<bool> EmailAndResetPasswordHashAreValidAsync(
            string emailAddress,
            string resetHash)
        {
            var matchingResetPasswordEntities =
                await passwordResetDataService.FindMatchingResetPasswordEntitiesWithUserDetailsAsync(
                    emailAddress,
                    resetHash);

            return matchingResetPasswordEntities.Any(
                resetPassword => resetPassword.IsStillValidAt(clockService.UtcNow));
        }

        public void GenerateAndSendPasswordResetLink(string emailAddress, string baseUrl)
        {
            (User? user, List<DelegateUser> delegateUsers) = userService.GetUsersByEmailAddress(emailAddress);
            user ??= delegateUsers.FirstOrDefault() ??
                     throw new UserAccountNotFoundException(
                         "No user account could be found with the specified email address");
            string resetPasswordHash = GenerateResetPasswordHash(user);
            var resetPasswordEmail = GeneratePasswordResetEmail(
                emailAddress,
                resetPasswordHash,
                user.FirstName,
                baseUrl);
            emailService.SendEmail(resetPasswordEmail);
        }

        public async Task InvalidateResetPasswordForEmailAsync(string email)
        {
            var resetPasswordIds = this.userService.GetUsersByEmailAddress(email).GetDistinctResetPasswordIds();

            foreach (var resetPasswordId in resetPasswordIds)
            {
                await this.passwordResetDataService.RemoveResetPasswordAsync(resetPasswordId);
            }
        }

        public async Task ChangePasswordAsync(string email, string newPassword)
        {
            var hashOfPassword = this.cryptoService.GetPasswordHash(newPassword);
            await this.passwordDataService.SetPasswordByEmailAsync(email, hashOfPassword);
        }

        private string GenerateResetPasswordHash(User user)
        {
            string hash = Guid.NewGuid().ToString();

            var resetPasswordCreateModel = new ResetPasswordCreateModel(
                clockService.UtcNow,
                hash,
                user.Id,
                user is DelegateUser ? UserType.DelegateUser : UserType.AdminUser);

            passwordResetDataService.CreatePasswordReset(resetPasswordCreateModel);

            return hash;
        }

        private static Email GeneratePasswordResetEmail(
            string emailAddress,
            string resetHash,
            string firstName,
            string baseUrl)
        {
            UriBuilder resetPasswordUrl = new UriBuilder(baseUrl);
            if (!resetPasswordUrl.Path.EndsWith('/'))
            {
                resetPasswordUrl.Path += '/';
            }

            resetPasswordUrl.Path += "ResetPassword";
            resetPasswordUrl.Query = $"code={resetHash}&email={emailAddress}";

            string emailSubject = "Digital Learning Solutions Tracking System Password Reset";

            var body = new BodyBuilder
            {
                TextBody = $@"Dear {firstName},
                            A request has been made to reset the password for your Digital Learning Solutions account.
                            To reset your password please follow this link: {resetPasswordUrl.Uri}
                            Note that this link can only be used once and it will expire in two hours.
                            Please don’t reply to this email as it has been automatically generated.",
                HtmlBody = $@"<body style= 'font - family: Calibri; font - size: small;'>
                                    <p>Dear {firstName},</p>
                                    <p>A request has been made to reset the password for your Digital Learning Solutions account.</p>
                                    <p>To reset your password please follow this link: <a href=""{resetPasswordUrl.Uri}"">{resetPasswordUrl.Uri}</a></p>
                                    <p>Note that this link can only be used once and it will expire in two hours.</p>
                                    <p>Please don’t reply to this email as it has been automatically generated.</p>
                                </body>"
            };

            return new Email(emailSubject, body, emailAddress);
        }
    }
}
