namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using DigitalLearningSolutions.Data.Models.User;

    public static class ResetPasswordTestHelpers
    {
        public static IEnumerable<ResetPassword> GetResetPasswordById(this DbConnection connection, int resetPasswordId)
        {
            return connection.Query<ResetPassword>(
                "SELECT * FROM ResetPassword WHERE ID = @ResetPasswordId",
                new { ResetPasswordId = resetPasswordId }
            );
        }

        public static async Task<int> GetResetPasswordIdByHashAsync(this DbConnection connection, string hash)
        {
            var resetPasswordId = (await connection.QueryAsync<int>(
                "SELECT Id FROM ResetPassword WHERE ResetPasswordHash = @Hash;",
                new { Hash = hash }
            )).Single();
            return resetPasswordId;
        }

        public static async Task SetResetPasswordIdForUserAsync(
            this DbConnection connection,
            UserAccount user,
            int resetPasswordId
        )
        {
            await connection.ExecuteAsync(
                $"UPDATE Users SET ResetPasswordId = @ResetPasswordId WHERE ID = @UserId;",
                new { ResetPasswordId = resetPasswordId, UserId = user.Id }
            );
        }
    }
}
