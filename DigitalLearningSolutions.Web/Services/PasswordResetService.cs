﻿namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Auth;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using FreshdeskApi.Client.Contacts.Models;
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

        void GenerateAndSendDelegateWelcomeEmail(int delegateId, string baseUrl, string registrationConfirmationHash);

        void GenerateAndScheduleDelegateWelcomeEmail(
            int delegateId,
            string baseUrl,
            DateTime deliveryDate,
            string addedByProcess
        );

        void SendWelcomeEmailsToDelegates(
            IEnumerable<int> delegateId,
            DateTime deliveryDate,
            string baseUrl
        );

        Email GenerateDelegateWelcomeEmail(int delegateId, string baseUrl);
        Email GenerateEmailInviteForCentreManager(
           string centreName,
           string email,
           string baseUrl,
           string SupportEmail
       );
    }

    public class PasswordResetService : IPasswordResetService
    {
        private readonly IClockUtility clockUtility;
        private readonly IEmailService emailService;
        private readonly IPasswordResetDataService passwordResetDataService;
        private readonly IRegistrationConfirmationDataService registrationConfirmationDataService;
        private readonly IPasswordService passwordService;
        private readonly IUserService userService;

        public PasswordResetService(
            IUserService userService,
            IPasswordResetDataService passwordResetDataService,
            IRegistrationConfirmationDataService registrationConfirmationDataService,
            IPasswordService passwordService,
            IEmailService emailService,
            IClockUtility clockUtility
        )
        {
            this.userService = userService;
            this.passwordResetDataService = passwordResetDataService;
            this.registrationConfirmationDataService = registrationConfirmationDataService;
            this.passwordService = passwordService;
            this.emailService = emailService;
            this.clockUtility = clockUtility;
        }

        public async Task GenerateAndSendPasswordResetLink(string emailAddress, string baseUrl)
        {
            var user = userService.GetUserAccountByEmailAddress(emailAddress);

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

            var resetPasswordHash = GenerateResetPasswordHash(user.Id);

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

        public void GenerateAndSendDelegateWelcomeEmail(int delegateId, string baseUrl, string registrationConfirmationHash)
        {
            var delegateEntity = userService.GetDelegateById(delegateId)!;

            var welcomeEmail = GenerateWelcomeEmail(
                delegateEntity,
                registrationConfirmationHash,
                baseUrl
            );
            emailService.SendEmail(welcomeEmail);
        }

        public Email GenerateDelegateWelcomeEmail(int delegateId, string baseUrl)
        {
            var delegateEntity = userService.GetDelegateById(delegateId)!;
            var welcomeEmail = GenerateWelcomeEmail(
                delegateEntity,
                delegateEntity.DelegateAccount.RegistrationConfirmationHash,
                baseUrl
            );
            return welcomeEmail;
        }

        public void GenerateAndScheduleDelegateWelcomeEmail(
            int delegateId,
            string baseUrl,
            DateTime deliveryDate,
            string addedByProcess
        )
        {
            var delegateEntity = userService.GetDelegateById(delegateId)!;

            var registrationConfirmationHash = GenerateRegistrationConfirmationHash(delegateId);
            var welcomeEmail = GenerateWelcomeEmail(
                delegateEntity,
                registrationConfirmationHash,
                baseUrl
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
                resetPasswordEntity != null && resetPasswordEntity.IsStillValidAt(clockUtility.UtcNow, expiryTime)
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
            IEnumerable<int> delegateIds,
            DateTime deliveryDate,
            string baseUrl
        )
        {
            const string addedByProcess = "SendWelcomeEmail_Refactor";
            var emails = delegateIds.Select(
                delegateId =>
                {
                    var delegateEntity = userService.GetDelegateById(delegateId)!;
                    return GenerateWelcomeEmail(
                        delegateEntity,
                        GenerateRegistrationConfirmationHash(delegateId),
                        baseUrl
                    );
                }
            );
            emailService.ScheduleEmails(emails, addedByProcess, deliveryDate);
        }
        public Email GenerateEmailInviteForCentreManager(
           string centreName,
           string email,
           string baseUrl,
           string SupportEmail
       )
        {
            var emailInvite = GenerateEmailInvite(
                centreName,
                email,
                baseUrl,
                SupportEmail
            );
            return emailInvite;
        }
        private string GenerateResetPasswordHash(int userId)
        {
            var hash = Guid.NewGuid().ToString();

            var resetPasswordCreateModel = new ResetPasswordCreateModel(
                clockUtility.UtcNow,
                hash,
                userId
            );

            passwordResetDataService.CreatePasswordReset(resetPasswordCreateModel);

            return hash;
        }

        private string GenerateRegistrationConfirmationHash(int delegateId)
        {
            var hash = Guid.NewGuid().ToString();

            var registrationConfirmationModel = new RegistrationConfirmationModel(
                clockUtility.UtcNow,
                hash,
                delegateId
            );

            registrationConfirmationDataService.SetRegistrationConfirmation(registrationConfirmationModel);

            return hash;
        }

        private static Email GeneratePasswordResetEmail(
            string emailAddress,
            string resetHash,
            string fullName,
            string baseUrl
        )
        {
            var resetPasswordUrl = new UriBuilder(baseUrl);
            if (!resetPasswordUrl.Path.EndsWith('/'))
            {
                resetPasswordUrl.Path += '/';
            }

            resetPasswordUrl.Path += "ResetPassword";
            resetPasswordUrl.Query = $"code={resetHash}&email={HttpUtility.UrlEncode(emailAddress)}";

            var emailSubject = "Digital Learning Solutions Tracking System Password Reset";

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

        private static Email GenerateWelcomeEmail(
            DelegateEntity delegateEntity,
            string registrationConfirmationHash,
            string baseUrl
        )
        {
            var emailAddress = delegateEntity.EmailForCentreNotifications;
            var completeRegistrationUrl = new UriBuilder(baseUrl);
            if (!completeRegistrationUrl.Path.EndsWith('/'))
            {
                completeRegistrationUrl.Path += '/';
            }

            completeRegistrationUrl.Path += "ClaimAccount";
            completeRegistrationUrl.Query =
                $"code={registrationConfirmationHash}&email={HttpUtility.UrlEncode(emailAddress)}";

            const string emailSubject = "Welcome to Digital Learning Solutions - Verify your Registration";

            var body = new BodyBuilder
            {
                TextBody = $@"Dear {delegateEntity.UserAccount.FullName},%0D%0DAn administrator has registered your details to give you access to the Digital Learning Solutions (DLS) platform under the centre {delegateEntity.DelegateAccount.CentreName}.%0D%0DYou have been assigned the unique DLS delegate number {delegateEntity.DelegateAccount.CandidateNumber}.%0D%0DTo complete your registration and access your Digital Learning Solutions content, please click: {completeRegistrationUrl.Uri}%0D%0DNote that this link can only be used once.%0D%0DPlease don't reply to this email as it has been automatically generated.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear {delegateEntity.UserAccount.FullName},</p>
                                <p>An administrator has registered your details to give you access to the Digital Learning Solutions (DLS) platform under the centre {delegateEntity.DelegateAccount.CentreName}.</p>
                                <p>You have been assigned the unique DLS delegate number {delegateEntity.DelegateAccount.CandidateNumber}.</p>
                                <p><a href=""{completeRegistrationUrl.Uri}"">Click here to complete your registration and access your Digital Learning Solutions content</a></p>
                                <p>Note that this link can only be used once.</p>
                                <p>Please don't reply to this email as it has been automatically generated.</p>
                            </body>",
            };
            return new Email(emailSubject, body, emailAddress);
        }

        private static Email GenerateEmailInvite(
           string centreName,
           string email,
           string baseUrl,
           string SupportEmail
       )
        {
            var completeRegistrationUrl = new UriBuilder(baseUrl);
            const string emailSubject = "Welcome to the Digital Learning Solutions (DLS) Platform";

            var body = new BodyBuilder
            {
                TextBody = $@"Dear Colleague,%0D%0DYour centre,  {centreName}, has been successfully registered on the  Digital Learning Solutions (DLS), and you’ve been pre-registered as the Centre Manager.%0D%0DTo activate your Centre Manager and Learner accounts, please complete your registration by selecting the link below:%0D%0DComplete Your Registration {completeRegistrationUrl.Uri}%0D%0DPlease use {email} during the registration process to ensure it’s successful.%0D%0DFor any questions or assistance, contact us at {SupportEmail}.%0D%0DKind regards,%0D%0D DLS Support Team",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear Colleague,</p>
                                <p>Your centre, {centreName}, has been successfully registered on the Digital Learning Solutions (DLS), and you’ve been pre-registered as the Centre Manager.</p>
                                <p>To activate your Centre Manager and Learner accounts, please complete your registration by selecting the link below:</p>
                                <p><a href=""{completeRegistrationUrl.Uri}"">Complete Your Registration </a></p>
                                <p>Please use {email} during the registration process to ensure it’s successful.</p>
                                <p>For any questions or assistance, contact us at {SupportEmail}.</p>
                                <p>Kind regards,</p>
                                <p>DLS Support Team</p>
                            </body>",
            };
            return new Email(emailSubject, body, email);
        }
    }
}
