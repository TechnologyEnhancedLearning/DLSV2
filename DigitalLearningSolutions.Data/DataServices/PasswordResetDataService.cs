namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Auth;
    using Microsoft.Extensions.Logging;

    public interface IPasswordResetDataService
    {
        Task<List<ResetPasswordWithUserDetails>> FindMatchingResetPasswordEntitiesWithUserDetailsAsync(
            string userEmail,
            string resetHash);

        void CreatePasswordReset(ResetPasswordCreateModel createModel);
        Task RemoveResetPasswordAsync(int resetPasswordId);
    }

    public class PasswordResetDataService : IPasswordResetDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<PasswordResetDataService> logger;

        public PasswordResetDataService(
            IDbConnection connection,
            ILogger<PasswordResetDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public async Task<List<ResetPasswordWithUserDetails>> FindMatchingResetPasswordEntitiesWithUserDetailsAsync(
            string userEmail,
            string resetHash)
        {
            var matches = await connection.QueryAsync<ResetPasswordWithUserDetails>(
                @"SELECT RPAU.UserId, RPAU.Email, RPAU.ID, RPAU.ResetPasswordHash, RPAU.PasswordResetDateTime, RPAU.UserType
FROM (SELECT AU.AdminID     UserId,
             AU.Email,
             RP.ID,
             RP.ResetPasswordHash,
             RP.PasswordResetDateTime,
             'AdminUser' as UserType
      FROM dbo.AdminUsers AU
               JOIN [ResetPassword] RP ON AU.ResetPasswordID = RP.ID
      WHERE AU.Email = @userEmail
        AND RP.ResetPasswordHash = @resetHash) RPAU
UNION
SELECT C.CandidateID,
       C.EmailAddress,
       RP.ID,
       RP.ResetPasswordHash,
       RP.PasswordResetDateTime,
       'DelegateUser' as UserType
FROM dbo.Candidates C
         JOIN [ResetPassword] RP ON C.ResetPasswordID = RP.ID
WHERE C.EmailAddress = @userEmail
  AND RP.ResetPasswordHash = @resetHash;",
                new { userEmail, resetHash }
            );
            return matches.ToList();
        }

        public void CreatePasswordReset(ResetPasswordCreateModel createModel)
        {
            var numberOfAffectedRows = connection.Execute(
                GetCreateResetPasswordSql(createModel.UserType),
                new
                {
                    ResetPasswordHash = createModel.Hash,
                    CreateTime = createModel.CreateTime,
                    UserID = createModel.UserId,
                });
            if (numberOfAffectedRows < 2)
            {
                string message =
                    $"Not saving reset password hash as db insert/update failed for User ID: {createModel.UserId} from table {createModel.UserType.TableName}";
                logger.LogWarning(message);
                throw new ResetPasswordInsertException(message);
            }
        }

        public async Task RemoveResetPasswordAsync(int resetPasswordId)
        {
            await connection.ExecuteAsync(@"BEGIN TRY
                        BEGIN TRANSACTION
                            UPDATE AdminUsers SET ResetPasswordID = null WHERE ResetPasswordID = @ResetPasswordId;
                            UPDATE Candidates SET ResetPasswordID = null WHERE ResetPasswordID = @ResetPasswordId;
                            DELETE FROM ResetPassword WHERE ID = @ResetPasswordId;
                        COMMIT TRANSACTION
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRANSACTION
                    END CATCH",
                new { ResetPasswordId = resetPasswordId });
        }

        private static string GetCreateResetPasswordSql(UserType userType)
        {
            return $@"BEGIN TRY
                        DECLARE @ResetPasswordID INT
                        BEGIN TRANSACTION
                            INSERT INTO dbo.ResetPassword
                                ([ResetPasswordHash]
                                ,[PasswordResetDateTime])
                            VALUES(@ResetPasswordHash, @CreateTime)

                            SET @ResetPasswordID = SCOPE_IDENTITY()

                            UPDATE {userType.TableName}
                            SET ResetPasswordID = @ResetPasswordID
                            WHERE {userType.IdColumnName} = @UserID
                        COMMIT TRANSACTION
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRANSACTION
                    END CATCH
                    ";
        }
    }
}
