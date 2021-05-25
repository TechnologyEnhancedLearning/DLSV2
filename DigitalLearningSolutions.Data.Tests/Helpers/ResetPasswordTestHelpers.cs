namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Data.Common;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.DbModels;

    public static class ResetPasswordTestHelpers
    {

        public static IEnumerable<ResetPassword> GetResetPasswordById(this DbConnection connection, int resetPasswordId)
        {
            return connection.Query<ResetPassword>
            (
                "SELECT * FROM ResetPassword WHERE ID = @ResetPasswordId",
                new { ResetPasswordId = resetPasswordId }
            );
        }

    }
}
