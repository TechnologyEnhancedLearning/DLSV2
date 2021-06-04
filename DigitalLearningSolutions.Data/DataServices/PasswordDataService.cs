namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IPasswordDataService
    {
        Task SetPasswordByUserReferenceAsync(UserReference userRef, string passwordHash);
        void SetPasswordByCandidateNumber(string candidateNumber, string passwordHash);
        Task SetPasswordByEmailAsync(string email, string passwordHash);
    }

    public class PasswordDataService : IPasswordDataService
    {
        private readonly IDbConnection connection;

        public PasswordDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task SetPasswordByUserReferenceAsync(UserReference userRef, string passwordHash)
        {
            await connection.ExecuteAsync(
                $@"UPDATE {userRef.UserType.TableName}
                    SET Password = @Password
                    WHERE {userRef.UserType.IdColumnName} = @UserId;",
                new { Password = passwordHash, UserId = userRef.Id }
            );
        }

        public void SetPasswordByCandidateNumber(string candidateNumber, string passwordHash)
        {
            connection.Query(
                @"UPDATE Candidates
                        SET Password = @passwordHash
                        WHERE CandidateNumber = @candidateNumber",
                new { passwordHash, candidateNumber }
            );
        }

        public async Task SetPasswordByEmailAsync(
            string email,
            string passwordHash
        )
        {
            await connection.ExecuteAsync(
                @"BEGIN TRY
                    BEGIN TRANSACTION
                        UPDATE AdminUsers SET Password = @PasswordHash WHERE Email = @Email;
                        UPDATE Candidates SET Password = @PasswordHash WHERE EmailAddress = @Email;
                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH",
                new { Email = email, PasswordHash = passwordHash }
            );
        }
    }
}
