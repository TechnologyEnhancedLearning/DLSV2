namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    public interface IPasswordResetService
    {
        void GenerateAndSendPasswordResetLink(string emailAddress, string baseUrl);
    }

    public class PasswordResetService : IPasswordResetService
    {
        private const string AdminTableName = "AdminUsers";
        private const string DelegatesTableName = "Candidates";
        private const string AdminIdColumnName = "AdminID";
        private const string DelegatesIdColumnName = "CandidateID";
        private readonly IDbConnection connection;
        private readonly IEmailService emailService;
        private readonly ILogger<PasswordResetService> logger;

        private readonly IUserService userService;

        public PasswordResetService(
            IUserService userService,
            IDbConnection connection,
            ILogger<PasswordResetService> logger,
            IEmailService emailService)
        {
            this.userService = userService;
            this.connection = connection;
            this.logger = logger;
            this.emailService = emailService;
        }

        public void GenerateAndSendPasswordResetLink(string emailAddress, string baseUrl)
        {
            (User? user, List<DelegateUser> delegateUsers) = userService.GetUsersByEmailAddress(emailAddress);
            user ??= delegateUsers.FirstOrDefault() ?? throw new UserAccountNotFoundException("No user account could be found with the specified email address");
            string resetPasswordHash = GenerateResetPasswordHash(user);
            Email resetPasswordEmail = GeneratePasswordResetEmail(emailAddress, resetPasswordHash, user.FirstName, baseUrl);
            emailService.SendEmail(resetPasswordEmail);
        }

        private string GenerateResetPasswordHash(User user)
        {
            string hash = Guid.NewGuid().ToString();
            string tableName = user.GetType() == typeof(DelegateUser) ? DelegatesTableName : AdminTableName;
            string idColumnName = user.GetType() == typeof(DelegateUser) ? DelegatesIdColumnName : AdminIdColumnName;

            var numberOfAffectedRows = connection.Execute(
                $@"BEGIN TRY
                        DECLARE @ResetPasswordID INT
                        BEGIN TRANSACTION
                            INSERT INTO dbo.ResetPassword
                                ([ResetPasswordHash]
                                ,[PasswordResetDateTime])
                            VALUES(@ResetPasswordHash, GETDATE())

                            SET @ResetPasswordID = SCOPE_IDENTITY()

                            UPDATE {tableName}
                            SET ResetPasswordID = @ResetPasswordID
                            WHERE {idColumnName} = @UserID
                        COMMIT TRANSACTION
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRANSACTION
                    END CATCH
                    ", new { ResetPasswordHash = hash, UserID = user.Id });
            if (numberOfAffectedRows < 2)
            {
                string message = $"Not saving reset password hash as db insert/update failed for User ID: {user.Id} from table {tableName}";
                logger.LogWarning(message);
                throw new ResetPasswordInsertException(message);
            }

            return hash;
        }

        private Email GeneratePasswordResetEmail(string emailAddress, string resetHash, string firstName, string baseUrl)
        {
            UriBuilder resetPasswordUrl = new UriBuilder(baseUrl);
            if (!resetPasswordUrl.Path.EndsWith('/'))
            {
                resetPasswordUrl.Path += '/';
            }
            resetPasswordUrl.Path += "ResetPassword";
            resetPasswordUrl.Query = $"code={resetHash}&email={emailAddress}";
            
            string emailSubject = "Digital Learning Solutions Tracking System Password Reset";

            BodyBuilder body = new BodyBuilder
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
