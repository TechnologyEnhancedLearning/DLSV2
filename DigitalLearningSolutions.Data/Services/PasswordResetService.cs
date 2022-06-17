namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Auth;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using MimeKit;

    public interface IPasswordResetService
    {
        Task<ResetPasswordWithUserDetails?> GetValidPasswordResetEntityAsync(
            string emailAddress,
            string resetHash,
            TimeSpan expiryTime
        );

        Task<bool> EmailAndResetPasswordHashAreValidAsync(
            string emailAddress,
            string resetHash,
            TimeSpan expiryTime
        );

        Task GenerateAndSendPasswordResetLink(string emailAddress, string baseUrl);

        Task ResetPasswordAsync(ResetPasswordWithUserDetails passwordReset, string password);

        void GenerateAndSendDelegateWelcomeEmail(string emailAddress, string candidateNumber, string baseUrl);

        void GenerateAndScheduleDelegateWelcomeEmail(
            string recipientEmailAddress,
            string candidateNumber,
            string baseUrl,
            DateTime deliveryDate,
            string addedByProcess
        );

        void SendWelcomeEmailsToDelegates(
            IEnumerable<DelegateUser> delegateUsers,
            DateTime deliveryDate,
            string baseUrl
        );
    }

    public class PasswordResetService : IPasswordResetService
    {
        private readonly IClockService clockService;
        private readonly IEmailService emailService;
        private readonly IPasswordResetDataService passwordResetDataService;
        private readonly IPasswordService passwordService;
        private readonly IUserService userService;

        public PasswordResetService(
            IUserService userService,
            IPasswordResetDataService passwordResetDataService,
            IPasswordService passwordService,
            IEmailService emailService,
            IClockService clockService
        )
        {
            this.userService = userService;
            this.passwordResetDataService = passwordResetDataService;
            this.passwordService = passwordService;
            this.emailService = emailService;
            this.clockService = clockService;
        }

        public async Task GenerateAndSendPasswordResetLink(string emailAddress, string baseUrl)
        {
            var user = userService.GetUserByEmailAddress(emailAddress);

            if (user == null)
            {
                throw new UserAccountNotFoundException(
                    "No user account could be found with the specified email address"
                );
            }

            if (user.ResetPasswordId != null)
            {
                await passwordResetDataService.RemoveResetPasswordAsync(user.ResetPasswordId.Value);
            }

            string resetPasswordHash = GenerateResetPasswordHash(user);

            var resetPasswordEmail = GeneratePasswordResetEmail(
                emailAddress,
                resetPasswordHash,
                user.FullName,
                baseUrl
            );
            emailService.SendEmail(resetPasswordEmail);
        }

        public async Task ResetPasswordAsync(ResetPasswordWithUserDetails passwordReset, string password)
        {
            await passwordResetDataService.RemoveResetPasswordAsync(passwordReset.Id);
            await passwordService.ChangePasswordAsync(passwordReset.UserId, password!);
            userService.ResetFailedLoginCountByUserId(passwordReset.UserId);
        }

        public void GenerateAndSendDelegateWelcomeEmail(string emailAddress, string candidateNumber, string baseUrl)
        {
            var delegateUsers = userService.GetDelegateUsersByEmailAddress(emailAddress);
            var delegateUser = delegateUsers.FirstOrDefault(d => d.CandidateNumber == candidateNumber) ??
                               throw new UserAccountNotFoundException(
                                   "No user account could be found with the specified email address and candidate number"
                               );

            string setPasswordHash = GenerateResetPasswordHash(delegateUser);
            var welcomeEmail = GenerateWelcomeEmail(
                emailAddress,
                setPasswordHash,
                baseUrl,
                delegateUser
            );
            emailService.SendEmail(welcomeEmail);
        }

        public void GenerateAndScheduleDelegateWelcomeEmail(
            string recipientEmailAddress,
            string candidateNumber,
            string baseUrl,
            DateTime deliveryDate,
            string addedByProcess
        )
        {
            var delegateUsers = userService.GetDelegateUsersByEmailAddress(recipientEmailAddress);
            var delegateUser = delegateUsers.FirstOrDefault(d => d.CandidateNumber == candidateNumber) ??
                               throw new UserAccountNotFoundException(
                                   "No user account could be found with the specified email address and candidate number"
                               );

            string setPasswordHash = GenerateResetPasswordHash(delegateUser);
            var welcomeEmail = GenerateWelcomeEmail(
                recipientEmailAddress,
                setPasswordHash,
                baseUrl,
                delegateUser
            );

            emailService.ScheduleEmail(welcomeEmail, addedByProcess, deliveryDate);
        }

        public async Task<ResetPasswordWithUserDetails?> GetValidPasswordResetEntityAsync(
            string emailAddress,
            string resetHash,
            TimeSpan expiryTime
        )
        {
            var resetPasswordEntity =
                await passwordResetDataService.FindMatchingResetPasswordEntityWithUserDetailsAsync(
                    emailAddress,
                    resetHash
                );

            return
                resetPasswordEntity != null && resetPasswordEntity.IsStillValidAt(clockService.UtcNow, expiryTime)
                    ? resetPasswordEntity
                    : null;
        }

        public async Task<bool> EmailAndResetPasswordHashAreValidAsync(
            string emailAddress,
            string resetHash,
            TimeSpan expiryTime
        )
        {
            return await GetValidPasswordResetEntityAsync(emailAddress, resetHash, expiryTime) != null;
        }

        public void SendWelcomeEmailsToDelegates(
            IEnumerable<DelegateUser> delegateUsers,
            DateTime deliveryDate,
            string baseUrl
        )
        {
            const string addedByProcess = "SendWelcomeEmail_Refactor";
            var emails = delegateUsers.Select(
                delegateUser =>
                    GenerateWelcomeEmail(
                        delegateUser.EmailAddress!,
                        GenerateResetPasswordHash(delegateUser),
                        baseUrl,
                        delegateUser
                    )
            );
            emailService.ScheduleEmails(emails, addedByProcess, deliveryDate);
        }

        private string GenerateResetPasswordHash(UserAccount user)
        {
            return GenerateResetPasswordHash(user.Id);
        }

        private string GenerateResetPasswordHash(DelegateUser delegateUser)
        {
            return GenerateResetPasswordHash(delegateUser.Id); // TODO: HEEDLS-887 delegateUser.Id != userId, so this is the wrong id to use here.
        }

        private string GenerateResetPasswordHash(int userId)
        {
            string hash = Guid.NewGuid().ToString();

            var resetPasswordCreateModel = new ResetPasswordCreateModel(
                clockService.UtcNow,
                hash,
                userId
            );

            passwordResetDataService.CreatePasswordReset(resetPasswordCreateModel);

            return hash;
        }

        private static Email GeneratePasswordResetEmail(
            string emailAddress,
            string resetHash,
            string fullName,
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

            var body = new BodyBuilder
            {
                TextBody = $@"Dear {fullName},
                            A request has been made to reset the password for your Digital Learning Solutions account.
                            To reset your password please follow this link: {resetPasswordUrl.Uri}
                            Note that this link can only be used once and it will expire in two hours.
                            Please don’t reply to this email as it has been automatically generated.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                    <p>Dear {fullName},</p>
                                    <p>A request has been made to reset the password for your Digital Learning Solutions account.</p>
                                    <p>To reset your password please follow this link: <a href=""{resetPasswordUrl.Uri}"">{resetPasswordUrl.Uri}</a></p>
                                    <p>Note that this link can only be used once and it will expire in two hours.</p>
                                    <p>Please don’t reply to this email as it has been automatically generated.</p>
                                </body>",
            };

            return new Email(emailSubject, body, emailAddress);
        }

        private Email GenerateWelcomeEmail(
            string emailAddress,
            string setPasswordHash,
            string baseUrl,
            DelegateUser delegateUser
        )
        {
            UriBuilder setPasswordUrl = new UriBuilder(baseUrl);
            if (!setPasswordUrl.Path.EndsWith('/'))
            {
                setPasswordUrl.Path += '/';
            }

            setPasswordUrl.Path += "SetPassword"; // TODO: HEEDLS-901 The controller for this link has been deleted
            setPasswordUrl.Query = $"code={setPasswordHash}&email={emailAddress}";

            const string emailSubject = "Welcome to Digital Learning Solutions - Verify your Registration";

            BodyBuilder body = new BodyBuilder
            {
                TextBody = $@"Dear {delegateUser.FullName},
                            An administrator has registered your details to give you access to the Digital Learning Solutions (DLS) platform under the centre {delegateUser.CentreName}.
                            You have been assigned the unique DLS delegate number {delegateUser.CandidateNumber}.
                            To complete your registration and access your Digital Learning Solutions content, please click: {setPasswordUrl.Uri}
                            Note that this link can only be used once and it will expire in three days.
                            Please don't reply to this email as it has been automatically generated.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear {delegateUser.FullName},</p>
                                <p>An administrator has registered your details to give you access to the Digital Learning Solutions (DLS) platform under the centre {delegateUser.CentreName}.</p>
                                <p>You have been assigned the unique DLS delegate number {delegateUser.CandidateNumber}.</p>
                                <p><a href=""{setPasswordUrl.Uri}"">Click here to complete your registration and access your Digital Learning Solutions content</a></p>
                                <p>Note that this link can only be used once and it will expire in three days.</p>
                                <p>Please don't reply to this email as it has been automatically generated.</p>
                            </body>",
            };

            return new Email(emailSubject, body, emailAddress);
        }
    }
}
