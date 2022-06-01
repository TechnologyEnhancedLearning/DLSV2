namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;

    public interface IPasswordDataService
    {
        void SetPasswordByCandidateNumber(string candidateNumber, string passwordHash);

        Task SetPasswordByUserIdAsync(int userId, string passwordHash);

        Task SetOldPasswordsToNullByUserIdAsync(int userId);
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
                @"UPDATE Users
                        SET PasswordHash = @passwordHash
                        FROM Users
                        INNER JOIN DelegateAccounts AS d ON d.UserID = Users.ID
                        WHERE d.CandidateNumber = @candidateNumber",
                new { passwordHash, candidateNumber }
            );
        }

        public async Task SetPasswordByUserIdAsync(int userId, string passwordHash)
        {
            await connection.ExecuteAsync(
                @"UPDATE Users
                        SET PasswordHash = @passwordHash
                        WHERE Users.ID = @userId",
                new { userId, passwordHash }
            );
        }

        public async Task SetOldPasswordsToNullByUserIdAsync(int userId)
        {
            await connection.ExecuteAsync(
                @"UPDATE DelegateAccounts
                        SET OldPassword = NULL
                        WHERE UserId = @userId",
                new { userId }
            );
        }
    }
}
