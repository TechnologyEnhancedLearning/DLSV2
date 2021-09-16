namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public interface ISupervisorDelegateDataService
    {
        SupervisorDelegate? GetSupervisorDelegateRecord(int supervisorDelegateId);
    }

    public class SupervisorDelegateDataService : ISupervisorDelegateDataService
    {
        private readonly IDbConnection connection;

        public SupervisorDelegateDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public SupervisorDelegate? GetSupervisorDelegateRecord(int supervisorDelegateId)
        {
            return connection.QuerySingle<SupervisorDelegate>(
                @"SELECT
                        ID,
                        SupervisorAdminID,
                        SupervisorEmail,
                        CandidateID,
                        DelegateEmail,
                        Added,
                        AddedByDelegate,
                        NotificationSent,
                        Confirmed,
                        Removed
                    FROM SupervisorDelegates
                    WHERE (ID = @supervisorDelegateId) AND (Removed IS NULL)",
                new { supervisorDelegateId }
            );
        }
    }
}
