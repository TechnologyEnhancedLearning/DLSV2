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

        public async Task SetPasswordForUsersAsync(IEnumerable<UserReference> users, string passwordHash)
        {
            var userRefs = users.ToList();
            await connection.ExecuteAsync(
                @"UPDATE AdminUsers SET Password = @PasswordHash WHERE AdminID IN (@AdminIds);
                  UPDATE Candidates SET Password = @PasswordHash WHERE CandidateID IN (@CandidateIds);",
                new
                {
                    PasswordHash = passwordHash,
                    AdminIds = userRefs.Where(ur => ur.UserType.Equals(UserType.AdminUser)).Select(ur => ur.Id),
                    CandidateIds = userRefs.Where(ur => ur.UserType.Equals(UserType.DelegateUser)).Select(ur => ur.Id),
                }
            );
        }
    }
}
