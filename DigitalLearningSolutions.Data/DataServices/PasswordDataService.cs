namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IPasswordDataService
    {
        void SetPasswordByCandidateNumber(string candidateNumber, string passwordHash);

        Task SetPasswordByEmailAsync(string email, string passwordHash);

        Task SetPasswordForUsersAsync(IEnumerable<UserReference> users, string passwordHash);
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

        public async Task SetPasswordByEmailAsync(
            string email,
            string passwordHash
        )
        {
            await connection.ExecuteAsync(
                @"BEGIN TRY
                    BEGIN TRANSACTION
                        UPDATE Users
                        SET PasswordHash = @passwordHash
                        FROM Users
                            LEFT JOIN DelegateAccounts AS d ON d.UserID = Users.ID
	                        LEFT JOIN AdminAccounts AS a ON a.UserID = Users.ID
                        WHERE Users.PrimaryEmail = @email
                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH",
                new { email, passwordHash }
            );
        }

        public async Task SetPasswordForUsersAsync(IEnumerable<UserReference> users, string passwordHash)
        {
            var userRefs = users.ToList();

            await connection.ExecuteAsync(
                @"UPDATE Users
                        SET PasswordHash = @PasswordHash
                        FROM Users
                            LEFT JOIN DelegateAccounts AS d ON d.UserID = Users.ID
	                        LEFT JOIN AdminAccounts AS a ON a.UserID = Users.ID
                        WHERE a.ID IN @AdminIds OR d.ID IN @DelegateIds",
                new
                {
                    PasswordHash = passwordHash,
                    AdminIds = userRefs.Where(ur => ur.UserType.Equals(UserType.AdminUser)).Select(ur => ur.Id),
                    DelegateIds = userRefs.Where(ur => ur.UserType.Equals(UserType.DelegateUser)).Select(ur => ur.Id),
                }
            );
        }
    }
}
