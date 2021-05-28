namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;

    public interface IPasswordDataService
    {
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

        public void SetPasswordByCandidateNumber(string candidateNumber, string passwordHash)
        {
            connection.Query(
                @"UPDATE Candidates
                        SET Password = @passwordHash
                        WHERE CandidateNumber = @candidateNumber",
                new { passwordHash, candidateNumber });
        }

        public async Task SetPasswordByEmailAsync(
            string email,
            string passwordHash)
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
                new { Email = email, PasswordHash = passwordHash });
        }
    }
}
