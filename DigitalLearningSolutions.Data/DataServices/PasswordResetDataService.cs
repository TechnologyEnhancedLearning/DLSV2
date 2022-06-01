namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Auth;
    using Microsoft.Extensions.Logging;

    public interface IPasswordResetDataService
    {
        Task<ResetPasswordWithUserDetails?> FindMatchingResetPasswordEntityWithUserDetailsAsync(
            string userEmail,
            string resetHash
        );

        void CreatePasswordReset(ResetPasswordCreateModel createModel);

        Task RemoveResetPasswordAsync(int resetPasswordId);
    }

    public class PasswordResetDataService : IPasswordResetDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<PasswordResetDataService> logger;

        public PasswordResetDataService(
            IDbConnection connection,
            ILogger<PasswordResetDataService> logger
        )
        {
            this.connection = connection;
            this.logger = logger;
        }

        public async Task<ResetPasswordWithUserDetails?> FindMatchingResetPasswordEntityWithUserDetailsAsync(
            string userEmail,
            string resetHash
        )
        {
            return await connection.QuerySingleOrDefaultAsync<ResetPasswordWithUserDetails>(
                @"
                    SELECT
                        u.ID           AS UserId,
                        u.PrimaryEmail AS Email,
                        rp.ID          AS Id,
                        rp.ResetPasswordHash,
                        rp.PasswordResetDateTime
                    FROM Users u
                    JOIN ResetPassword rp ON u.ResetPasswordID = rp.ID
                    WHERE u.PrimaryEmail = @userEmail
                    AND rp.ResetPasswordHash = @resetHash;",
                new { userEmail, resetHash }
            );
        }

        public void CreatePasswordReset(ResetPasswordCreateModel createModel)
        {
            var numberOfAffectedRows = connection.Execute(
                @"
                    BEGIN TRY
                    DECLARE @ResetPasswordID INT
                    BEGIN TRANSACTION
                        INSERT INTO dbo.ResetPassword
                            ([ResetPasswordHash]
                            ,[PasswordResetDateTime])
                        VALUES(@ResetPasswordHash, @CreateTime)

                        SET @ResetPasswordID = SCOPE_IDENTITY()

                        UPDATE Users
                        SET ResetPasswordID = @ResetPasswordID
                        WHERE ID = @UserID
                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
                ",
                new
                {
                    ResetPasswordHash = createModel.Hash,
                    CreateTime = createModel.CreateTime,
                    UserID = createModel.UserId,
                }
            );
            if (numberOfAffectedRows < 2)
            {
                string message =
                    $"Not saving reset password hash as db insert/update failed for User ID: {createModel.UserId}";
                logger.LogWarning(message);
                throw new ResetPasswordInsertException(message);
            }
        }

        public async Task RemoveResetPasswordAsync(int resetPasswordId)
        {
            await connection.ExecuteAsync(
                @"BEGIN TRY
                        BEGIN TRANSACTION
                            UPDATE Users SET ResetPasswordID = null WHERE ResetPasswordID = @resetPasswordId;
                            DELETE FROM ResetPassword WHERE ID = @resetPasswordId;
                        COMMIT TRANSACTION
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRANSACTION
                    END CATCH",
                new { resetPasswordId }
            );
        }
    }
}
