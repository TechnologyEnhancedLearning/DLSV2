namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System;
    using Castle.Core.Internal;

    public static class ServiceTestHelper
    {
        public static string GetJenkinsSqlConnectionString()
        {
            var jenkinsSqlServerPassword = Environment.GetEnvironmentVariable("SqlTestCredentials_PSW");
            var jenkinsSqlServerUsername = Environment.GetEnvironmentVariable("SqlTestCredentials_USR");
            return jenkinsSqlServerUsername.IsNullOrEmpty() || jenkinsSqlServerPassword.IsNullOrEmpty()
                ? ""
                : $"Server=HEE-DLS-SQL\\HEETEST; Database=mbdbx101_test; User Id={jenkinsSqlServerUsername}; Password={jenkinsSqlServerPassword};";
        }

    }
}
