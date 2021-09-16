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
            return connection.QuerySingle<SupervisorDelegate?>(
                @"SELECT
                        sd.ID,
                        sd.SupervisorAdminID,
                        sd.SupervisorEmail,
                        sd.CandidateID,
                        sd.DelegateEmail,
                        sd.Added,
                        sd.AddedByDelegate,
                        sd.NotificationSent,
                        sd.Confirmed,
                        sd.Removed,
                        au.CentreID
                    FROM SupervisorDelegates sd
                    INNER JOIN AdminUsers au ON sd.SupervisorAdminID = au.AdminID
                    WHERE ID = @supervisorDelegateId",
                new { supervisorDelegateId }
            );
        }
    }
}
