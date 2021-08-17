namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Auth;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using MimeKit;

    public interface IPasswordResetService
    {
        Task<bool> EmailAndResetPasswordHashAreValidAsync(
            string emailAddress,
            string resetHash,
            bool isSetPassword
        );

        void GenerateAndSendPasswordResetLink(string emailAddress, string baseUrl);
        Task InvalidateResetPasswordForEmailAsync(string email);
        void GenerateAndSendDelegateWelcomeEmail(string emailAddress, string baseUrl);
    }

    public class PasswordResetService : IPasswordResetService
    {
        private readonly IClockService clockService;
        private readonly IEmailService emailService;
        private readonly IPasswordResetDataService passwordResetDataService;

        private readonly IUserService userService;

        public PasswordResetService(
            IUserService userService,
            IPasswordResetDataService passwordResetDataService,
            IEmailService emailService,
            IClockService clockService
        )
        {
            this.userService = userService;
            this.passwordResetDataService = passwordResetDataService;
            this.emailService = emailService;
            this.clockService = clockService;
        }

        public async Task<bool> EmailAndResetPasswordHashAreValidAsync(
            string emailAddress,
            string resetHash,
            bool isSetPassword
        )
        {
            var matchingResetPasswordEntities =
                await passwordResetDataService.FindMatchingResetPasswordEntitiesWithUserDetailsAsync(
                    emailAddress,
                    resetHash
                );

            return matchingResetPasswordEntities.Any(
                resetPassword => resetPassword.IsStillValidAt(
                    clockService.UtcNow,
                    isSetPassword
                        ? ResetPasswordHelpers.SetPasswordHasExpiryTime
                        : ResetPasswordHelpers.ResetPasswordHashExpiryTime
                )
            );
        }

        public void GenerateAndSendPasswordResetLink(string emailAddress, string baseUrl)
        {
            (User? user, List<DelegateUser> delegateUsers) = userService.GetUsersByEmailAddress(emailAddress);
            user ??= delegateUsers.FirstOrDefault() ??
                     throw new UserAccountNotFoundException(
                         "No user account could be found with the specified email address"
                     );
            string resetPasswordHash = GenerateResetPasswordHash(user);
            var resetPasswordEmail = GeneratePasswordResetEmail(
                emailAddress,
                resetPasswordHash,
                user.FirstName,
                baseUrl
            );
            emailService.SendEmail(resetPasswordEmail);
        }

        public async Task InvalidateResetPasswordForEmailAsync(string email)
        {
            var resetPasswordIds = userService.GetUsersByEmailAddress(email).GetDistinctResetPasswordIds();

            foreach (var resetPasswordId in resetPasswordIds)
            {
                await passwordResetDataService.RemoveResetPasswordAsync(resetPasswordId);
            }
        }

        public void GenerateAndSendDelegateWelcomeEmail(string emailAddress, string baseUrl)
        {
            (_, List<DelegateUser> delegateUsers) = userService.GetUsersByEmailAddress(emailAddress);
            var delegateUser = delegateUsers.FirstOrDefault() ??
                     throw new UserAccountNotFoundException(
                         "No user account could be found with the specified email address"
                     );

            string setPasswordHash = GenerateResetPasswordHash(delegateUser);
            var welcomeEmail = GenerateWelcomeEmail(
                emailAddress,
                setPasswordHash,
                baseUrl,
                delegateUser.FirstName,
                delegateUser.LastName,
                delegateUser.CentreName,
                delegateUser.CandidateNumber
            );
            emailService.SendEmail(welcomeEmail);
        }

        private string GenerateResetPasswordHash(User user)
        {
            string hash = Guid.NewGuid().ToString();

            var resetPasswordCreateModel = new ResetPasswordCreateModel(
                clockService.UtcNow,
                hash,
                user.Id,
                user is DelegateUser ? UserType.DelegateUser : UserType.AdminUser
            );

            passwordResetDataService.CreatePasswordReset(resetPasswordCreateModel);

            return hash;
        }

        private static Email GeneratePasswordResetEmail(
            string emailAddress,
            string resetHash,
            string? firstName,
            string baseUrl
        )
        {
            UriBuilder resetPasswordUrl = new UriBuilder(baseUrl);
            if (!resetPasswordUrl.Path.EndsWith('/'))
            {
                resetPasswordUrl.Path += '/';
            }

            resetPasswordUrl.Path += "ResetPassword";
            resetPasswordUrl.Query = $"code={resetHash}&email={emailAddress}";

            string emailSubject = "Digital Learning Solutions Tracking System Password Reset";

            var nameToUse = firstName ?? "User";

            var body = new BodyBuilder
            {
                TextBody = $@"Dear {nameToUse},
                            A request has been made to reset the password for your Digital Learning Solutions account.
                            To reset your password please follow this link: {resetPasswordUrl.Uri}
                            Note that this link can only be used once and it will expire in two hours.
                            Please don’t reply to this email as it has been automatically generated.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                    <p>Dear {nameToUse},</p>
                                    <p>A request has been made to reset the password for your Digital Learning Solutions account.</p>
                                    <p>To reset your password please follow this link: <a href=""{resetPasswordUrl.Uri}"">{resetPasswordUrl.Uri}</a></p>
                                    <p>Note that this link can only be used once and it will expire in two hours.</p>
                                    <p>Please don’t reply to this email as it has been automatically generated.</p>
                                </body>"
            };

            return new Email(emailSubject, body, emailAddress);
        }

        private Email GenerateWelcomeEmail(
            string emailAddress,
            string setPasswordHash,
            string baseUrl,
            string? firstName,
            string lastName,
            string centreName,
            string candidateNumber
        )
        {
            UriBuilder setPasswordUrl = new UriBuilder(baseUrl);
            if (!setPasswordUrl.Path.EndsWith('/'))
            {
                setPasswordUrl.Path += '/';
            }

            setPasswordUrl.Path += "SetPassword";
            setPasswordUrl.Query = $"code={setPasswordHash}&email={emailAddress}";

            const string emailSubject = "Welcome to Digital Learning Solutions - Verify your Registration";

            var nameToUse = firstName != null ? $"{firstName} {lastName}" : lastName;

            BodyBuilder body = new BodyBuilder
            {
                TextBody = $@"Dear {nameToUse},
                            An administrator has registered your details to give you access to the Digital Learning Solutions (DLS) platform under the centre {centreName}.
                            You have been assigned the unique DLS delegate number {candidateNumber}.
                            To complete your registration and access your Digital Learning Solutions content, please click: {setPasswordUrl.Uri}
                            Note that this link can only be used once and it will expire in three days.
                            Please don't reply to this email as it has been automatically generated.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear {nameToUse},</p>
                                <p>An administrator has registered your details to give you access to the Digital Learning Solutions (DLS) platform under the centre {centreName}.</p>
                                <p>You have been assigned the unique DLS delegate number {candidateNumber}.</p>
                                <p>To complete your registration and access your Digital Learning Solutions content, please click <a href=""{setPasswordUrl.Uri}"">this link</a>.</p>
                                <p>Note that this link can only be used once and it will expire in three days.</p>
                                <p>Please don't reply to this email as it has been automatically generated.</p>
                            </body>"
            };

            return new Email(emailSubject, body, emailAddress);
        }
    }
}
