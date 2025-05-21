namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using System.Data;

    public interface ILoginDataService
    {
        void UpdateLastAccessedForUsersTable(int Id);

        void UpdateLastAccessedForDelegatesAccountsTable(int Id);

        void UpdateLastAccessedForAdminAccountsTable(int Id);
    }

    public class LoginDataService : ILoginDataService
    {
        private readonly IDbConnection connection;

        public LoginDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public void UpdateLastAccessedForUsersTable(int Id)
        {
            connection.Execute(
                @"UPDATE Users SET
                        LastAccessed = GetUtcDate()
                WHERE ID = @Id",
                new
                {
                    Id
                }
            );
        }
        
        public void UpdateLastAccessedForDelegatesAccountsTable(int Id)
        {
            connection.Execute(
                @"UPDATE DelegateAccounts SET
                        LastAccessed = GetUtcDate()
                WHERE ID = @Id",
                new
                {
                    Id
                }
            );
        }

        public void UpdateLastAccessedForAdminAccountsTable(int Id)
        {
            connection.Execute(
                @"UPDATE AdminAccounts SET
                        LastAccessed = GetUtcDate()
                WHERE ID = @Id",
                new
                {
                    Id
                }
            );
        }
    }
}
