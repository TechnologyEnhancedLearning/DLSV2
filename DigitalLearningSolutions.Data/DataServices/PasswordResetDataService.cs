namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using Dapper.Contrib.Extensions;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Auth;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using Microsoft.Extensions.Logging;

    public interface IPasswordResetDataService
    {
        Task<ResetPassword> FindAsync(int id);
        void CreatePasswordReset(ResetPasswordCreateModel createModel);
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

        public async Task<ResetPassword> FindAsync(int id)
        {
            return await connection.GetAsync<ResetPassword>(id);
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
