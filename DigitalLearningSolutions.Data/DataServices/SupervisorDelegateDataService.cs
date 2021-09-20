namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public interface ISupervisorDelegateDataService
    {
        SupervisorDelegate? GetSupervisorDelegateRecord(int supervisorDelegateId);
        SupervisorDelegate? GetPendingSupervisorDelegateRecordByEmail(int centreId, string email);
        void UpdateSupervisorDelegateRecordStatus(int supervisorDelegateId, int candidateId, DateTime? confirmed);
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
            return connection.QuerySingleOrDefault<SupervisorDelegate?>(
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

        public SupervisorDelegate? GetPendingSupervisorDelegateRecordByEmail(int centreId, string email)
        {
            return connection.QuerySingleOrDefault<SupervisorDelegate?>(
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
                    WHERE au.CentreID = @centreId
                      AND sd.DelegateEmail = @email
                      AND sd.CandidateID IS NULL
                      AND sd.Removed IS NULL",
                new { centreId, email }
            );
        }

        public void UpdateSupervisorDelegateRecordStatus(int supervisorDelegateId, int candidateId, DateTime? confirmed)
        {
            connection.Execute(
                @"UPDATE SupervisorDelegates
                    SET CandidateID = @candidateId,
                        Confirmed = @confirmed
                    WHERE ID = @supervisorDelegateId",
                new { supervisorDelegateId, candidateId, confirmed }
            );
        }
    }
}
